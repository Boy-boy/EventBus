using EventBus.Abstraction;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.RabbitMQ
{
    public class EventBusRabbitMq : EventBusBase, IDisposable
    {
        const string EXCHANGE_NAME = "event_bus_rabbitmq_default_exchange";
        const string QUEUE_NAME = "event_bus_rabbitmq_default_queue";
        private readonly IRabbitMqPersistentConnection _persistentConnection;
        private readonly IRabbitMqMessageConsumerFactory _rabbitMqMessageConsumerFactory;
        private readonly IEventBusSubscriptionsManager _subsManager;
        private readonly IEventHandlerFactory _eventHandlerFactory;
        private readonly ILogger<EventBusRabbitMq> _logger;
        private readonly EventBusRabbitMqOptions _eventBusRabbitMqOptions;
        private readonly int _retryCount = 5;
        private readonly object _lock = new object();
        protected IRabbitMqMessageConsumer RabbitMqMessageConsumer { get; private set; }


        public EventBusRabbitMq(
            IRabbitMqPersistentConnection persistentConnection,
            IRabbitMqMessageConsumerFactory rabbitMqMessageConsumerFactory,
            IEventBusSubscriptionsManager subsManager,
            IEventHandlerFactory eventHandlerFactory,
            ILogger<EventBusRabbitMq> logger,
            IOptions<EventBusRabbitMqOptions> options)
        {
            _eventBusRabbitMqOptions = options.Value;
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _rabbitMqMessageConsumerFactory = rabbitMqMessageConsumerFactory;
            _subsManager = subsManager ?? throw new ArgumentNullException(nameof(subsManager));
            _eventHandlerFactory = eventHandlerFactory ?? throw new ArgumentNullException(nameof(eventHandlerFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subsManager.OnEventRemoved += SubsManager_OnEventRemoved;
        }

        private void SubsManager_OnEventRemoved(object sender, string eventName)
        {
            RabbitMqMessageConsumer?.UnbindAsync(eventName);
        }

        protected override Task PublishAsync(Type eventType, IntegrationEvent eventDate)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }
            var policy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning(ex, "Could not publish event: {EventId} after {Timeout}s ({ExceptionMessage})", eventDate.Id, $"{time.TotalSeconds:n1}", ex.Message);
                });

            var eventName = EventNameAttribute.GetNameOrDefault(eventType);
            _logger.LogTrace("Creating RabbitMQ channel to publish event: {EventId} ({EventName})", eventDate.Id, eventName);

            using (var channel = _persistentConnection.CreateModel())
            {
                _logger.LogTrace("Declaring RabbitMQ exchange to publish event: {EventId}", eventDate.Id);
                var message = JsonConvert.SerializeObject(eventDate);
                var body = Encoding.UTF8.GetBytes(message);

                var model = channel;
                model.ExchangeDeclare(exchange: _eventBusRabbitMqOptions.RabbitMqPublishConfigure.ExchangeName, type: "direct", durable: true);
                policy.Execute(() =>
                {
                    var properties = model.CreateBasicProperties();
                    properties.DeliveryMode = 2; // persistent

                    _logger.LogTrace("Publishing event to RabbitMQ: {EventId}", eventDate.Id);

                    model.BasicPublish(
                        exchange: _eventBusRabbitMqOptions.RabbitMqPublishConfigure.ExchangeName,
                        routingKey: eventName,
                        mandatory: true,
                        basicProperties: properties,
                        body: body);
                });
            }
            return Task.CompletedTask;
        }

        protected override void Subscribe(Type eventType, Type handlerType)
        {
            SetMessageConsumer(eventType);
            var eventName = EventNameAttribute.GetNameOrDefault(eventType);
            if (!_subsManager.IncludeSubscriptionsHandlesForEventName(eventName))
                RabbitMqMessageConsumer?.BindAsync(eventName);
            _subsManager.AddSubscription(eventType, handlerType);

        }

        protected override void UnSubscribe(Type eventType, Type handlerType)
        {
            var eventName = EventNameAttribute.GetNameOrDefault(eventType);
            _logger.LogInformation("Unsubscribing from event {EventName}", eventName);
            _subsManager.RemoveSubscription(eventType, handlerType);
        }

        public void Dispose()
        {
            RabbitMqMessageConsumer?.Dispose();
            _persistentConnection?.Dispose();
            _subsManager?.Dispose();
        }

        private void SetMessageConsumer(Type eventType)
        {
            var rabbitMqSubscribeConfigure = _eventBusRabbitMqOptions.RabbitSubscribeConfigures.Find(p => p.EventType == eventType);
            if (rabbitMqSubscribeConfigure != null)
            {
                RabbitMqMessageConsumer = _rabbitMqMessageConsumerFactory.Create(
                    new RabbitMqExchangeDeclareConfigure(_eventBusRabbitMqOptions.ExchangeName, "direct", true),
                    new RabbitMqQueueDeclareConfigure(_eventBusRabbitMqOptions.QueueName));
                RabbitMqMessageConsumer.OnMessageReceived(Consumer_Received);
            }

            if (RabbitMqMessageConsumer != null) return;
            lock (_lock)
            {
                if (RabbitMqMessageConsumer != null) return;
                RabbitMqMessageConsumer = _rabbitMqMessageConsumerFactory.Create(
                    new RabbitMqExchangeDeclareConfigure(_eventBusRabbitMqOptions.ExchangeName, "direct", true),
                    new RabbitMqQueueDeclareConfigure(_eventBusRabbitMqOptions.QueueName));
                RabbitMqMessageConsumer.OnMessageReceived(Consumer_Received);
            }
        }

        private string GetPublishConfigure()
        {
            return string.IsNullOrEmpty(_eventBusRabbitMqOptions.RabbitMqPublishConfigure.ExchangeName)
                ? EXCHANGE_NAME
                : _eventBusRabbitMqOptions.RabbitMqPublishConfigure.ExchangeName;
        }

        private (string, string) GetSubscribeConfigures(Type eventType)
        {
            var subscribeConfigure = _eventBusRabbitMqOptions.RabbitSubscribeConfigures.Find(p => p.EventType == eventType);

            return ("", "");
        }

        private async Task Consumer_Received(IModel model, BasicDeliverEventArgs eventArgs)
        {
            var eventName = eventArgs.RoutingKey;
            var message = Encoding.UTF8.GetString(eventArgs.Body);
            try
            {
                if (message.ToLowerInvariant().Contains("throw-fake-exception"))
                {
                    throw new InvalidOperationException($"Fake exception requested: \"{message}\"");
                }
                await ProcessEvent(eventName, message);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "----- ERROR Processing message \"{Message}\"", message);
            }
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            _logger.LogTrace("Processing RabbitMQ event: {eventName}", eventName);

            if (_subsManager.IncludeEventTypeForEventName(eventName))
            {
                var eventHandleTypes = _subsManager.GetHandlerTypes(eventName);
                foreach (var eventHandleType in eventHandleTypes)
                {
                    var handlerInstance = _eventHandlerFactory.GetHandler(eventHandleType);
                    var eventType = _subsManager.TryGetEventTypeForEventName(eventName);
                    var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                    var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                    await Task.Yield();
                    await (Task)concreteType.GetMethod("HandleAsync").Invoke(handlerInstance, new[] { integrationEvent });
                }
            }
            else
            {
                _logger.LogWarning("No subscription for RabbitMQ event: {eventName}", eventName);
            }
        }
    }
}
