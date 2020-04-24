using System;

namespace EventBusRabbitMQ.Configures
{
    public class RabbitMqPublishConfigure
    {
        public Type EventType { get; set; }
        public string ExchangeName { get; set; }

        public RabbitMqPublishConfigure() { }
        public RabbitMqPublishConfigure(Type eventType, string exchangeName)
        {
            EventType = eventType;
            ExchangeName = exchangeName ?? throw new ArgumentNullException(nameof(exchangeName)); 
        }
    }
}
