using EventBus;
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
using System.Threading;
using System.Threading.Tasks;
using EventBus.Abstraction;

namespace EventBusRabbitMQ
{
    public class EventBusRabbitMq : IEventBus
    {
        const string EXCHANGE_NAME = "event_bus_rabbitmq_default_exchange";
        const string QUEUE_NAME = "event_bus_rabbitmq_default_queue";
        private readonly IRabbitMqPersistentConnection _persistentConnection;
        private readonly IEventBusSubscriptionsManager _subsManager;
        private readonly IEventHandlerFactory _eventHandlerFactory;
        private readonly ILogger<EventBusRabbitMq> _logger;
        private readonly EventBusRabbitMqOptions _options;
        private readonly Dictionary<string, IModel> _consumerChannels;
        private readonly int _retryCount = 5;
        private Timer _timer;


        public EventBusRabbitMq(
            IRabbitMqPersistentConnection persistentConnection,
            IEventBusSubscriptionsManager subsManager,
            IEventHandlerFactory eventHandlerFactory,
            ILogger<EventBusRabbitMq> logger,
            IOptions<EventBusRabbitMqOptions> option)
        {
            _options = option.Value;
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _subsManager = subsManager ?? throw new ArgumentNullException(nameof(subsManager));
            _eventHandlerFactory = eventHandlerFactory ?? throw new ArgumentNullException(nameof(eventHandlerFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _consumerChannels = new Dictionary<string, IModel>();
            _subsManager.OnEventRemoved += SubsManager_OnEventRemoved;
        }
        private void SubsManager_OnEventRemoved(object sender, string eventName)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }
            using (var channel = _persistentConnection.CreateModel())
            {
                var eventType = _subsManager.TryGetEventTypeForEventName(eventName);
                var (exchangeName, queueName) = GetRabbitMqSubscribeExchangeNameAndQueueName(eventType);
                channel.QueueUnbind(queue: queueName,
                    exchange: exchangeName,
                    routingKey: eventName);
                if (!_consumerChannels.ContainsKey(queueName)) return;
                _consumerChannels[queueName]?.Close();
                _consumerChannels.Remove(queueName);
            }
        }

        #region Publish
        public Task Publish(IntegrationEvent @event)
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

            var eventName = EventNameAttribute.GetNameOrDefault(@event.GetType());
            _logger.LogTrace("Creating RabbitMQ channel to publish event: {EventId} ({EventName})", @event.Id, eventName);

            using (var channel = _persistentConnection.CreateModel())
            {
                _logger.LogTrace("Declaring RabbitMQ exchange to publish event: {EventId}", @event.Id);
                var message = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(message);
                var exchangeName = GetRabbitMqPublishExchangeName(@event.GetType());
                var model = channel;
                model.ExchangeDeclare(exchange: exchangeName, type: "direct", durable: true);
                policy.Execute(() =>
                {
                    var properties = model.CreateBasicProperties();
                    properties.DeliveryMode = 2; // persistent

                    _logger.LogTrace("Publishing event to RabbitMQ: {EventId}", @event.Id);

                    model.BasicPublish(
                        exchange: exchangeName,
                        routingKey: eventName,
                        mandatory: true,
                        basicProperties: properties,
                        body: body);
                });
            }
            return Task.CompletedTask;
        }
        #endregion

        #region Subscribe
        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventType = typeof(T);
            var eventName = EventNameAttribute.GetNameOrDefault(eventType);
            var (exchangeName, queueName) = GetRabbitMqSubscribeExchangeNameAndQueueName(eventType);
            TryBindQueue(exchangeName, queueName, eventName);
            _logger.LogInformation("Subscribing to event {EventName} with {EventHandler}", eventName, typeof(TH).Name);
            _subsManager.AddSubscription<T, TH>();
            StartBasicConsume(queueName);
            TrySetCustomerChannelTimer();
        }
        private void TryBindQueue(string exchangeName, string queueName, string eventName)
        {
            var includeHandles = _subsManager.IncludeSubscriptionsHandlesForEventName(eventName);
            if (includeHandles) return;
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }
            using (var channel = _persistentConnection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: exchangeName, type: "direct", durable: true);
                channel.QueueDeclare(queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
                channel.QueueBind(queue: queueName,
                    exchange: exchangeName,
                    routingKey: eventName);
            }
        }
        #endregion

        public void UnSubscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = EventNameAttribute.GetNameOrDefault(typeof(T));
            _logger.LogInformation("Unsubscribing from event {EventName}", eventName);
            _subsManager.RemoveSubscription<T, TH>();
        }

        private void TrySetCustomerChannelTimer()
        {
            if (_timer == null)
            {
                if (!_persistentConnection.IsConnected) return;
                _timer = new Timer(sender =>
                {
                    if (!_persistentConnection.IsConnected) return;
                    var consumerChannelsCopy = _consumerChannels
                        .ToDictionary(p => p.Key, p => p.Value);
                    foreach (var keyValuePair in consumerChannelsCopy)
                    {
                        if (keyValuePair.Value != null && keyValuePair.Value.IsOpen) continue;
                        _consumerChannels.Remove(keyValuePair.Key);
                        StartBasicConsume(keyValuePair.Key);
                    }
                }, this, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(20));
            }
        }

        public void Dispose()
        {
            _persistentConnection?.Dispose();
            _subsManager?.Dispose();
            if (_timer == null)
                return;
            _timer.Dispose();
            _timer = null;
        }

        private string GetRabbitMqPublishExchangeName(Type eventType)
        {
            var configure = _options.GetRabbitMqPublishConfigure(eventType);
            var exchangeName = configure?.ExchangeName ?? EXCHANGE_NAME;
            return exchangeName;
        }

        private (string exchangeName, string queueName) GetRabbitMqSubscribeExchangeNameAndQueueName(Type eventType)
        {
            var configure = _options.GetRabbitMqSubscribeConfigure(eventType);
            var exchangeName = configure?.ExchangeName ?? EXCHANGE_NAME;
            var queueName = configure?.QueueName ?? QUEUE_NAME;
            return (exchangeName, queueName);
        }

        private IModel CreateConsumerChannel(string queueName)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            _logger.LogTrace("Creating RabbitMQ consumer channel");

            var channel = _persistentConnection.CreateModel();
            channel.CallbackException += ConsumerChannelCallbackException;
            return channel;
            void ConsumerChannelCallbackException(object sender, CallbackExceptionEventArgs args)
            {
                if (!_persistentConnection.IsConnected)
                {
                    if (!_consumerChannels.ContainsKey(queueName)) return;
                    _consumerChannels[queueName].Dispose();
                    _consumerChannels[queueName].CallbackException -= ConsumerChannelCallbackException;
                    return;
                }
                _logger.LogWarning(args.Exception, "Recreating RabbitMQ consumer channel");
                if (_consumerChannels.ContainsKey(queueName))
                {
                    _consumerChannels[queueName]?.Dispose();
                    _consumerChannels[queueName] = CreateConsumerChannel(queueName);
                }
                StartBasicConsume(queueName);
            }
        }

        private void StartBasicConsume(string queueName)
        {
            _logger.LogTrace("Starting RabbitMQ basic consume");
            if (!_consumerChannels.ContainsKey(queueName))
                _consumerChannels.Add(queueName, CreateConsumerChannel(queueName));
            if (_consumerChannels.ContainsKey(queueName))
            {
                var consumerChannel = _consumerChannels[queueName];
                var consumer = new AsyncEventingBasicConsumer(consumerChannel);
                consumer.Received += Consumer_Received;
                consumerChannel.BasicQos(0, 50, false);
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
            var eventName = eventArgs.RoutingKey;
            var message = Encoding.UTF8.GetString(eventArgs.Body);
            var asyncEventingBasicConsumer = sender as AsyncEventingBasicConsumer;
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

            // Even on exception we take the message off the queue.
            // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
            // For more information see: https://www.rabbitmq.com/dlx.html
            asyncEventingBasicConsumer?.Model.BasicAck(eventArgs.DeliveryTag, multiple: false);
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
                    await (Task)concreteType.GetMethod("Handle").Invoke(handlerInstance, new[] { integrationEvent });
                }
            }
            else
            {
                _logger.LogWarning("No subscription for RabbitMQ event: {eventName}", eventName);
            }
        }
    }
}
