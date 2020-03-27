using System;

namespace EventBusRabbitMQ.Options
{
    public class RabbitMqPublishOption
    {
        public Type EventType { get; set; }
        public string ExchangeName { get; set; }

        public RabbitMqPublishOption() { }
        public RabbitMqPublishOption(Type eventType, string exchangeName)
        {
            EventType = eventType;
            ExchangeName = exchangeName;
        }
    }
}
