using System;
using System.Collections.Generic;

namespace EventBus.RabbitMQ
{
    public class EventBusRabbitMqOptions
    {
        public string QueueName { get; private set; }

        public string ExchangeName { get; private set; }

        public RabbitMqPublishConfigure RabbitMqPublishConfigure { get; set; }


        public List<RabbitMqSubscribeConfigure> RabbitSubscribeConfigures { get; set; }

        public EventBusRabbitMqOptions()
        {
            RabbitMqPublishConfigure = new RabbitMqPublishConfigure();
            RabbitSubscribeConfigures = new List<RabbitMqSubscribeConfigure>();
        }


        public EventBusRabbitMqOptions AddPublishConfigure(Action<RabbitMqPublishConfigure> configureBuilder)
        {
            if (configureBuilder == null) return this;
            configureBuilder.Invoke(RabbitMqPublishConfigure);
            return this;
        }

        public EventBusRabbitMqOptions AddSubscribeConfigures(Action<List<RabbitMqSubscribeConfigure>> configureOptions)
        {
            if (configureOptions == null) return this;
            configureOptions.Invoke(RabbitSubscribeConfigures);
            return this;
        }

        public EventBusRabbitMqOptions SetExchangeAndQueue(string exchangeName, string queueName)
        {
            if (string.IsNullOrEmpty(exchangeName))
                throw new ArgumentNullException(nameof(exchangeName));
            if (string.IsNullOrEmpty(queueName))
                throw new ArgumentNullException(nameof(queueName));
            ExchangeName = exchangeName;
            QueueName = queueName;
            return this;
        }
    }
}
