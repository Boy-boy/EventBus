using System;
using System.Collections.Generic;

namespace EventBus.RabbitMQ
{
    public class EventBusRabbitMqOptions
    {
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
    }
}
