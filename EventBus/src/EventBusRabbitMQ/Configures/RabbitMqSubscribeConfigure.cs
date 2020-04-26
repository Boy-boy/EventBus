using System;
using System.Collections.Generic;

namespace EventBusRabbitMQ.Configures
{
    public class RabbitMqSubscribeConfigureBuilder
    {
        public IList<RabbitMqSubscribeConfigure> RabbitMqSubscribeConfigures { get; set; } = new List<RabbitMqSubscribeConfigure>();

        public RabbitMqSubscribeConfigureBuilder AddRabbitMqSubscribeConfigure(Type eventType, string eventTag = default, string exchangeName = default, string queueName = default)
        {
            RabbitMqSubscribeConfigures.Add(new RabbitMqSubscribeConfigure(eventType, eventTag,exchangeName, queueName));
            return this;
        }
    }

    public class RabbitMqSubscribeConfigure
    {
        public Type EventType { get; set; }
        public string QueueName { get; set; }
        public string ExchangeName { get; set; }
        public string EventTag { get; set; }
        public RabbitMqSubscribeConfigure() { }
        public RabbitMqSubscribeConfigure(Type eventType, string eventTag = default, string exchangeName=default, string queueName=default)
        {
            EventType = eventType??throw new ArgumentNullException(nameof(eventType));
            ExchangeName = exchangeName;
            QueueName = queueName;
            EventTag = eventTag;
        }
    }
}
