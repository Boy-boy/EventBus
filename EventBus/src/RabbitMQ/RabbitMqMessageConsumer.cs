using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQ
{
    public class RabbitMqMessageConsumer : IRabbitMqMessageConsumer
    {
        private readonly ILogger<RabbitMqMessageConsumer> _logger;
        private readonly IRabbitMqPersistentConnection _persistentConnection;
        private readonly RabbitMqExchangeDeclareConfigure _exchangeDeclare;
        private readonly RabbitMqQueueDeclareConfigure _queueDeclare;
        protected IModel ConsumerChannel { get; private set; }
        private Timer _timer;

        protected ConcurrentBag<Func<IModel, BasicDeliverEventArgs, Task>> ProcessEvents { get; }

        public RabbitMqMessageConsumer(
            ILogger<RabbitMqMessageConsumer> logger,
            IRabbitMqPersistentConnection connection,
            IOptions<RabbitMqOptions> options)
        {
            var optionValue = options.Value;
            _exchangeDeclare = optionValue.ExchangeDeclare;
            _queueDeclare = optionValue.QueueDeclare;
            _logger = logger;
            _persistentConnection = connection;
            ProcessEvents = new ConcurrentBag<Func<IModel, BasicDeliverEventArgs, Task>>();
        }

        public Task BindAsync(string routingKey)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }
            if (ConsumerChannel == null || ConsumerChannel.IsClosed)
            {
                ConsumerChannel = _persistentConnection.CreateModel();
            }
            ConsumerChannel.ExchangeDeclare(
                    exchange: _exchangeDeclare.ExchangeName,
                    type: _exchangeDeclare.Type,
                    durable: _exchangeDeclare.Durable,
                    autoDelete: _exchangeDeclare.AutoDelete,
                    arguments: _exchangeDeclare.Arguments);
            ConsumerChannel.QueueDeclare(queue: _queueDeclare.QueueName,
                durable: _queueDeclare.Durable,
                exclusive: _queueDeclare.Exclusive,
                autoDelete: _queueDeclare.AutoDelete,
                arguments: _queueDeclare.Arguments);
            ConsumerChannel.QueueBind(queue: _queueDeclare.QueueName,
                exchange: _exchangeDeclare.ExchangeName,
                routingKey: routingKey);
            return Task.CompletedTask;
        }

        private void StartBasicConsume()
        {
            var consumer = new AsyncEventingBasicConsumer(ConsumerChannel);
            consumer.Received += Consumer_Received;
            ConsumerChannel.BasicQos(0, 50, false);
            ConsumerChannel.BasicConsume(
                queue: _queueDeclare.QueueName,
                autoAck: false,
                consumer: consumer);
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
        {
            var asyncEventingBasicConsumer = sender as AsyncEventingBasicConsumer;
            try
            {
                foreach (var processEvent in ProcessEvents)
                {
                    await processEvent(asyncEventingBasicConsumer?.Model, eventArgs);
                }
                // Even on exception we take the message off the queue.
                // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
                // For more information see: https://www.rabbitmq.com/dlx.html
                asyncEventingBasicConsumer?.Model.BasicAck(eventArgs.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        private void StartTimer()
        {
            if (!_persistentConnection.IsConnected) return;
            if (_timer == null)
            {
                _timer = new Timer(sender =>
                {
                    if (!_persistentConnection.IsConnected) return;
                    StartBasicConsume();
                }, this, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));
            }
        }

        public Task UnbindAsync(string routingKey)
        {
            throw new NotImplementedException();
        }
    }
}
