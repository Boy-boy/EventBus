using EventBus;
using EventBusRabbitMQ.Configures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
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
        private readonly ILogger<EventBusRabbitMq> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly EventBusRabbitMqOptions _options;
        private readonly IEventBusSubscriptionsManager _subsManager;
        private readonly int _retryCount = 5;
        private IModel _consumerChannel;

        public EventBusRabbitMq(IRabbitMqPersistentConnection persistentConnection,
            IEventBusSubscriptionsManager subsManager,
            ILogger<EventBusRabbitMq> logger,
            IServiceProvider serviceProvider,
            IOptions<EventBusRabbitMqOptions> option)
        {
            _options = option.Value;
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subsManager = subsManager ?? throw new ArgumentNullException(nameof(subsManager));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _subsManager.OnEventRemoved += SubsManager_OnEventRemoved;
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
                var option = _options.GetRabbitMqSubscribeConfigure(eventType);
                if (option == null)
                {
                    QueueUnbind(EXCHANGE_NAME, QUEUE_NAME);
                }
                else
                {
                    QueueUnbind(option.ExchangeName ?? EXCHANGE_NAME, option.QueueName ?? QUEUE_NAME);
                }
                void QueueUnbind(string exchangeName, string queueName)
                {
                    channel.QueueUnbind(queue: queueName,
                        exchange: exchangeName,
                        routingKey: eventKey);
                }
                if (!_subsManager.IsEmpty) return;
                _consumerChannel.Close();
            }
        }

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
                model.ExchangeDeclare(exchange: exchangeName, type: "direct");
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

        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = _subsManager.GetEventName<T>();
            var eventKey = _subsManager.GetEventKey<T>();
            DoInternalSubscription(eventKey, typeof(T));

            _logger.LogInformation("Subscribing to event {EventName} with {EventHandler}", eventName, typeof(TH).Name);

            _subsManager.AddSubscription<T, TH>();
            StartBasicConsume(typeof(T));
        }

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
            channel.CallbackException += (sender, ea) =>
            {
                _logger.LogWarning(ea.Exception, "Recreating RabbitMQ consumer channel");

                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel(eventType);
                StartBasicConsume(eventType);
            };

            return channel;
        }

        private void DoInternalSubscription(string eventKey, Type eventType)
        {
            var containsKey = _subsManager.HasSubscriptionsForEvent(eventKey);
            if (containsKey) return;
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }
            using (var channel = _persistentConnection.CreateModel())
            {
                var option = _options.GetRabbitMqSubscribeConfigure(eventType);
                if (option == null)
                {
                    QueueBind(EXCHANGE_NAME, QUEUE_NAME);
                }
                else
                {
                    QueueBind(option.ExchangeName ?? EXCHANGE_NAME, option.QueueName ?? QUEUE_NAME);
                }
                void QueueBind(string exchangeName, string queueName)
                {
                    channel.ExchangeDeclare(exchange: exchangeName, type: "direct");
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
        }

        public void Dispose()
        {
            _consumerChannel?.Dispose();

            _subsManager.Clear();
        }

        private void StartBasicConsume(Type eventType)
        {
            _logger.LogTrace("Starting RabbitMQ basic consume");

            if (_consumerChannel == null)
                _consumerChannel = CreateConsumerChannel(eventType);
            if (_consumerChannel != null)
            {
                var consumer = new AsyncEventingBasicConsumer(_consumerChannel);

                consumer.Received += Consumer_Received;
                var option = _options.GetRabbitMqSubscribeConfigure(eventType);
                BasicConsume(option == null ? QUEUE_NAME : option.QueueName ?? QUEUE_NAME);

                void BasicConsume(string queueName)
                {
                    _consumerChannel.BasicConsume(
                        queue: queueName,
                        autoAck: false,
                        consumer: consumer);
                }
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
            _consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }

        private async Task ProcessEvent(string eventKey, string message)
        {
            _logger.LogTrace("Processing RabbitMQ event: {EventKey}", eventKey);

            if (_subsManager.HasSubscriptionsForEvent(eventKey))
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var handlerTypes = _subsManager.GetHandlersForEvent(eventKey);
                    var serviceProvider = scope.ServiceProvider;
                    foreach (var handlerType in handlerTypes)
                    {
                        var handler = serviceProvider.GetService(handlerType);
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
