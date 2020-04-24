using System;

namespace EventBusRabbitMQ.Configures
{
    public class RabbitMqSubscribeConfigure
    {
        public Type EventType { get; set; }
        public string QueueName { get; set; }
        public string ExchangeName { get; set; }
        public string EventTag { get; set; }
        public RabbitMqSubscribeConfigure() { }
        public RabbitMqSubscribeConfigure(Type eventType, string exchangeName=default, string queueName=default,string eventTag=default)
        {
            EventType = eventType??throw new ArgumentNullException(nameof(eventType));
            ExchangeName = exchangeName;
            QueueName = queueName;
            EventTag = eventTag;
        }
    }
}
