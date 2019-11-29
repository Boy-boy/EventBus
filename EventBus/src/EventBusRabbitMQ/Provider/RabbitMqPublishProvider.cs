using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace EventBusRabbitMQ.Provider
{
    public class RabbitMqPublishProvider
    {
        private readonly IOptions<List<RabbitMqPublishOption>> _options;

        public RabbitMqPublishProvider(IOptions<List<RabbitMqPublishOption>> options)
        {
            _options = options;
        }
        public List<RabbitMqPublishOption> GetRabbitMqPublishOptions()
        {
            return _options.Value;
        }
    }

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
