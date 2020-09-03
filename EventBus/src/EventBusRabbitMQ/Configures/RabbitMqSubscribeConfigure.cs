﻿using System;
using System.Collections.Generic;

namespace EventBusRabbitMQ.Configures
{
    public class RabbitMqSubscribeConfigureBuilder
    {
        private List<RabbitMqSubscribeConfigure> RabbitMqSubscribeConfigures { get; } = new List<RabbitMqSubscribeConfigure>();

        public RabbitMqSubscribeConfigureBuilder AddRabbitMqSubscribeConfigure(Type eventType, string eventTag, string exchangeName = default, string queueName = default)
        {
            RabbitMqSubscribeConfigures.Add(new RabbitMqSubscribeConfigure(eventType, eventTag, exchangeName, queueName));
            return this;
        }

        public List<RabbitMqSubscribeConfigure> Build()
        {
            return RabbitMqSubscribeConfigures;
        }
    }

    public class RabbitMqSubscribeConfigure
    {
        public Type EventType { get; set; }
        public string EventTag { get; set; }
        public string QueueName { get; set; }
        public string ExchangeName { get; set; }
        public RabbitMqSubscribeConfigure() { }
        public RabbitMqSubscribeConfigure(Type eventType, string eventTag, string exchangeName = default, string queueName = default)
        {
            EventType = eventType ?? throw new ArgumentNullException(nameof(eventType));
            EventTag = eventTag ?? throw new ArgumentNullException(nameof(eventTag));
            ExchangeName = exchangeName;
            QueueName = queueName;
        }
    }
}
