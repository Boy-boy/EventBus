using System;
using System.Collections.Generic;

namespace EventBus.RabbitMQ.Configures
{

    public class RabbitMqPublishConfigureBuilder
    {
        private List<RabbitMqPublishConfigure> RabbitMqPublishConfigures { get;} = new List<RabbitMqPublishConfigure>();

        public RabbitMqPublishConfigureBuilder AddRabbitMqPublishConfigure(Type eventType, string exchangeName)
        {
            RabbitMqPublishConfigures.Add(new RabbitMqPublishConfigure(eventType, exchangeName));
            return this;
        }

        public List<RabbitMqPublishConfigure> Build()
        {
            return RabbitMqPublishConfigures;
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
