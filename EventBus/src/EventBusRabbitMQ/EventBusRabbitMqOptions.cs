using System;

namespace EventBus.RabbitMQ
{
    public class EventBusRabbitMqOptions
    {
        public string QueueName { get; private set; }

        public string ExchangeName { get; private set; }

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
