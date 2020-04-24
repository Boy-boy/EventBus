using System;
using EventBusRabbitMQ.Configures;
using System.Collections.Generic;
using System.Linq;

namespace EventBusRabbitMQ
{
    public class EventBusRabbitMqOption
    {
        public RabbitMqConnectionConfigure RabbitMqConnectionConfigures { get; private set; } = new RabbitMqConnectionConfigure();
        public List<RabbitMqPublishConfigure> RabbitMqPublishConfigures { get; private set; } = new List<RabbitMqPublishConfigure>();
        public List<RabbitMqSubscribeConfigure> RabbitMqSubscribeConfigures { get; private set; } = new List<RabbitMqSubscribeConfigure>();

        public EventBusRabbitMqOption ConfigRabbitMqConnectionConfigures(RabbitMqConnectionConfigure connectionConfigures)
        {
            RabbitMqConnectionConfigures = connectionConfigures ?? throw new ArgumentNullException(nameof(connectionConfigures));
            return this;
        }

        public EventBusRabbitMqOption ConfigRabbitMqPublishConfigures(List<RabbitMqPublishConfigure> publishConfigures)
        {
            if (publishConfigures == null)
                return this;
            RabbitMqPublishConfigures = publishConfigures;
            return this;
        }
        
        public EventBusRabbitMqOption ConfigRabbitMqSubscribeConfigures(List<RabbitMqSubscribeConfigure> subscribeConfigures)
        {
            if (subscribeConfigures == null)
                return this;
            RabbitMqSubscribeConfigures = subscribeConfigures;
            return this;
        }

        public RabbitMqPublishConfigure GetRabbitMqPublishConfigure(Type evenType)
        {
            RabbitMqPublishConfigure config = null;
            if (RabbitMqPublishConfigures != null &&
                RabbitMqPublishConfigures.Exists(p => p.EventType == evenType))
            {
                config = RabbitMqPublishConfigures.FirstOrDefault(p => p.EventType == evenType);
            }
            return config;
        }

        public RabbitMqSubscribeConfigure GetRabbitMqSubscribeConfigure(Type evenType)
        {
            RabbitMqSubscribeConfigure config = null;
            if (RabbitMqSubscribeConfigures != null &&
                RabbitMqSubscribeConfigures.Exists(p => p.EventType == evenType))
            {
                config = RabbitMqSubscribeConfigures.FirstOrDefault(p => p.EventType == evenType);
            }
            return config;
        }
    }
}
