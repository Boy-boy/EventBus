using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace EventBusRabbitMQ.Provider
{
    public class RabbitMqSubscribeProvider
    {
        private readonly IOptions<List<RabbitMqSubscribeOption>> _options;

        public RabbitMqSubscribeProvider(IOptions<List<RabbitMqSubscribeOption>> options)
        {
            _options = options;
        }

        public List<RabbitMqSubscribeOption> GetRabbitMqSubscribeOptions()
        {
            return _options.Value;
        }
    }
    public class RabbitMqSubscribeOption
    {
        public Type EventType { get; set; }
        public string QueueName { get; set; }
        public string ExchangeName { get; set; }
        public RabbitMqSubscribeOption() { }
        public RabbitMqSubscribeOption(Type eventType, string exchangeName, string queueName)
        {
            EventType = eventType;
            if(string.IsNullOrWhiteSpace(exchangeName)) throw new ArgumentNullException(nameof(exchangeName));
            if (string.IsNullOrWhiteSpace(queueName)) throw new ArgumentNullException(nameof(queueName));
            ExchangeName = exchangeName;
            QueueName = queueName;
        }
    }
}
