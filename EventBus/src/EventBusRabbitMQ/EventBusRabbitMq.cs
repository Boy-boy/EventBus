using EventBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EventBusRabbitMQ
{
    public class EventBusRabbitMq : IEventBus, IDisposable
    {
        const string EXCHANGE_NAME = "event_bus_rabbitmq_default_exchange";
        const string QUEUE_NAME = "event_bus_rabbitmq_default_queue";
        private readonly IRabbitMqPersistentConnection _persistentConnection;
        private readonly IEventBusSubscriptionsManager _subsManager;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<EventBusRabbitMq> _logger;
        private readonly EventBusRabbitMqOptions _options;
        private readonly Dictionary<string, IModel> _consumerChannels;
        private readonly Dictionary<string, List<Type>> _queueBindingEventTypes;
        private readonly int _retryCount = 5;


        public EventBusRabbitMq(IRabbitMqPersistentConnection persistentConnection,
            IEventBusSubscriptionsManager subsManager,
            ILogger<EventBusRabbitMq> logger,
            IServiceScopeFactory serviceScopeFactory,
            IOptions<EventBusRabbitMqOptions> option)
        {
            _options = option.Value;
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            _subsManager = subsManager ?? throw new ArgumentNullException(nameof(subsManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subsManager.OnEventRemoved += SubsManager_OnEventRemoved;
            _consumerChannels = new Dictionary<string, IModel>();
            _queueBindingEventTypes = new Dictionary<string, List<Type>>();
        }

        private void SubsManager_OnEventRemoved(object sender, string eventKey)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }
            using (var channel = _persistentConnection.CreateModel())
            {
                var eventType = _subsManager.GetEventTypeByKey(eventKey);
                var configure = _options.GetRabbitMqSubscribeConfigure(eventType);
                var exchangeName = configure?.ExchangeName ?? EXCHANGE_NAME;
                var queueName = configure?.QueueName ?? QUEUE_NAME;
                channel.QueueUnbind(queue: queueName,
                    exchange: exchangeName,
                    routingKey: eventKey);
                if (_queueBindingEventTypes.ContainsKey(queueName))
                {
                    _queueBindingEventTypes[queueName].Remove(eventType);
                    if (_queueBindingEventTypes[queueName].Any()) return;
                    _queueBindingEventTypes.Remove(queueName);
                    if (!_consumerChannels.ContainsKey(queueName)) return;
                    _consumerChannels[queueName].Close();
                    _consumerChannels.Remove(queueName);
                }
            }
        }

        #region Publish
        public void Publish(IntegrationEvent @event)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }
            var policy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning(ex, "Could not publish event: {EventId} after {Timeout}s ({ExceptionMessage})", @event.Id, $"{time.TotalSeconds:n1}", ex.Message);
                });

            var eventName = @event.GetType().Name;
            var routingKey = string.IsNullOrWhiteSpace(@event.EventTag) ? eventName : $"{@event.EventTag}_{eventName}";
            _logger.LogTrace("Creating RabbitMQ channel to publish event: {EventId} ({EventName})", @event.Id, eventName);

            using (var channel = _persistentConnection.CreateModel())
            {
                _logger.LogTrace("Declaring RabbitMQ exchange to publish event: {EventId}", @event.Id);
                var message = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(message);
                var exchangeName = _options.GetRabbitMqPublishConfigure(@event.GetType())?.ExchangeName ?? EXCHANGE_NAME;
                var model = channel;
                model.ExchangeDeclare(exchange: exchangeName, type: "direct", durable: true);
                policy.Execute(() =>
                {
                    var properties = model.CreateBasicProperties();
                    properties.DeliveryMode = 2; // persistent

                    _logger.LogTrace("Publishing event to RabbitMQ: {EventId}", @event.Id);

                    model.BasicPublish(
                        exchange: exchangeName,
                        routingKey: routingKey,
                        mandatory: true,
                        basicProperties: properties,
                        body: body);
                });
            }
        }
        #endregion

        #region Subscribe
        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            DoInternalSubscription<T>();
            DoAddQueueBindingEventTypes<T>();

            var eventName = _subsManager.GetEventName<T>();
            _logger.LogInformation("Subscribing to event {EventName} with {EventHandler}", eventName, typeof(TH).Name);

            _subsManager.AddSubscription<T, TH>();
            StartBasicConsume(typeof(T));
        }

        private void DoInternalSubscription<T>()
            where T : IntegrationEvent
        {
            var eventKey = _subsManager.GetEventKey<T>();
            var containsKey = _subsManager.HasSubscriptionsForEvent(eventKey);
            if (containsKey) return;
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }
            using (var channel = _persistentConnection.CreateModel())
            {
                var configure = _options.GetRabbitMqSubscribeConfigure(typeof(T));
                var exchangeName = configure?.ExchangeName ?? EXCHANGE_NAME;
                var queueName = configure?.QueueName ?? QUEUE_NAME;
                channel.ExchangeDeclare(exchange: exchangeName, type: "direct", durable: true);
                channel.QueueDeclare(queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
                channel.QueueBind(queue: queueName,
                    exchange: exchangeName,
                    routingKey: eventKey);
            }
        }

        private void DoAddQueueBindingEventTypes<T>()
            where T : IntegrationEvent
        {
            var eventType = typeof(T);
            var configure = _options.GetRabbitMqSubscribeConfigure(eventType);
            var queueName = configure?.QueueName ?? QUEUE_NAME;
            if (!_queueBindingEventTypes.ContainsKey(queueName))
            {
                _queueBindingEventTypes[queueName] = new List<Type>();
            }
            if (_queueBindingEventTypes[queueName].All(s => s != eventType))
            {
                _queueBindingEventTypes[queueName].Add(eventType);
            }
        }
        #endregion

        public void Unsubscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = _subsManager.GetEventName<T>();
            _logger.LogInformation("Unsubscribing from event {EventName}", eventName);
            _subsManager.RemoveSubscription<T, TH>();
        }

        private IModel CreateConsumerChannel(Type eventType)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            _logger.LogTrace("Creating RabbitMQ consumer channel");

            var channel = _persistentConnection.CreateModel();
            var option = _options.GetRabbitMqSubscribeConfigure(eventType);
            var queueName = option == null ? QUEUE_NAME : option.QueueName ?? QUEUE_NAME;
            channel.CallbackException += (sender, ea) =>
            {
                _logger.LogWarning(ea.Exception, "Recreating RabbitMQ consumer channel");
                if (_consumerChannels.ContainsKey(queueName))
                {
                    _consumerChannels[queueName].Dispose();
                    _consumerChannels[queueName] = CreateConsumerChannel(eventType);
                }
                StartBasicConsume(eventType);
            };

            return channel;
        }

     

        public void Dispose()
        {
            _consumerChannels.ToList().ForEach(p => p.Value.Dispose());

            _subsManager.Clear();
        }

        private void StartBasicConsume(Type eventType)
        {
            _logger.LogTrace("Starting RabbitMQ basic consume");
            var option = _options.GetRabbitMqSubscribeConfigure(eventType);
            var queueName =option?.QueueName ?? QUEUE_NAME;
            if (!_consumerChannels.ContainsKey(queueName))
                _consumerChannels.Add(queueName, CreateConsumerChannel(eventType));
            if (_consumerChannels.ContainsKey(queueName))
            {
                var consumerChannel = _consumerChannels[queueName];
                var consumer = new AsyncEventingBasicConsumer(consumerChannel);
                consumer.Received += Consumer_Received;
                consumerChannel.BasicConsume(
                    queue: queueName,
                    autoAck: false,
                    consumer: consumer);
            }
            else
            {
                _logger.LogError("StartBasicConsume can't call on _consumerChannel == null");
            }
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
        {
            var eventKey = eventArgs.RoutingKey;
            var message = Encoding.UTF8.GetString(eventArgs.Body);

            try
            {
                if (message.ToLowerInvariant().Contains("throw-fake-exception"))
                {
                    throw new InvalidOperationException($"Fake exception requested: \"{message}\"");
                }

                await ProcessEvent(eventKey, message);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "----- ERROR Processing message \"{Message}\"", message);
            }

            // Even on exception we take the message off the queue.
            // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
            // For more information see: https://www.rabbitmq.com/dlx.html
            //_consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }

        private async Task ProcessEvent(string eventKey, string message)
        {
            _logger.LogTrace("Processing RabbitMQ event: {EventKey}", eventKey);

            if (_subsManager.HasSubscriptionsForEvent(eventKey))
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var handlerTypes = _subsManager.GetHandlersForEvent(eventKey);
                    var serviceProvider = scope.ServiceProvider;
                    foreach (var handlerType in handlerTypes)
                    {
                        var handler = serviceProvider.GetRequiredService(handlerType);
                        if (handler == null) continue;
                        var eventType = _subsManager.GetEventTypeByKey(eventKey);
                        var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                        await Task.Yield();
                        await (Task)concreteType.GetMethod("Handle").Invoke(handler, new[] { integrationEvent });
                    }
                }
            }
            else
            {
                _logger.LogWarning("No subscription for RabbitMQ event: {EventKey}", eventKey);
            }
        }
    }
}
