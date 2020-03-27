using System;

namespace EventBusRabbitMQ.Options
{
    public class RabbitMqSubscribeOption
    {
        public Type EventType { get; set; }
        public string QueueName { get; set; }
        public string ExchangeName { get; set; }
        public RabbitMqSubscribeOption() { }
        public RabbitMqSubscribeOption(Type eventType, string exchangeName, string queueName)
        {
            EventType = eventType??throw new ArgumentNullException(nameof(eventType));
            if (string.IsNullOrWhiteSpace(exchangeName)) throw new ArgumentNullException(nameof(exchangeName));
            if (string.IsNullOrWhiteSpace(queueName)) throw new ArgumentNullException(nameof(queueName));
            ExchangeName = exchangeName;
            QueueName = queueName;
        }
    }
}
