using System;
using System.Collections.Generic;

namespace EventBusRabbitMQ.Configures
{

    public class RabbitMqPublishConfigureBuilder
    {
        public List<RabbitMqPublishConfigure> RabbitMqPublishConfigure { get;} = new List<RabbitMqPublishConfigure>();

        public RabbitMqPublishConfigureBuilder AddRabbitMqPublishConfigure(Type eventType, string exchangeName)
        {
            RabbitMqPublishConfigure.Add(new RabbitMqPublishConfigure(eventType, exchangeName));
            return this;
        }
    }

    public class RabbitMqPublishConfigure
    {
        public Type EventType { get; set; }
        public string ExchangeName { get; set; }

        public RabbitMqPublishConfigure() { }
        public RabbitMqPublishConfigure(Type eventType, string exchangeName)
        {
            EventType = eventType ?? throw new ArgumentNullException(nameof(eventType));
            ExchangeName = exchangeName ?? throw new ArgumentNullException(nameof(exchangeName)); 
        }
    }
}
