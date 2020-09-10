using EventBus.RabbitMQ.Configures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventBus.RabbitMQ
{
    public class EventBusRabbitMqOptions
    {
        private RabbitMqConnectionConfigure RabbitMqConnectionConfigure { get; set; } = new RabbitMqConnectionConfigure();
        private List<RabbitMqPublishConfigure> RabbitMqPublishConfigures { get; } = new List<RabbitMqPublishConfigure>();
        private List<RabbitMqSubscribeConfigure> RabbitMqSubscribeConfigures { get;} = new List<RabbitMqSubscribeConfigure>();

        public EventBusRabbitMqOptions ConfigRabbitMqConnectionConfigures(RabbitMqConnectionConfigure connectionConfigures)
        {
            RabbitMqConnectionConfigure = connectionConfigures ?? throw new ArgumentNullException(nameof(connectionConfigures));
            return this;
        }

        public EventBusRabbitMqOptions ConfigRabbitMqPublishConfigures(Action<RabbitMqPublishConfigureBuilder> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            var rabbitMqPublishConfigureBuilder = new RabbitMqPublishConfigureBuilder();
            builder(rabbitMqPublishConfigureBuilder);
            RabbitMqPublishConfigures.AddRange(rabbitMqPublishConfigureBuilder.Build());
            return this;
        }

        public EventBusRabbitMqOptions ConfigRabbitMqSubscribeConfigures(Action<RabbitMqSubscribeConfigureBuilder> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            var rabbitMqPublishConfigureBuilder = new RabbitMqSubscribeConfigureBuilder();
            builder(rabbitMqPublishConfigureBuilder);
            RabbitMqSubscribeConfigures.AddRange(rabbitMqPublishConfigureBuilder.Build());
            return this;
        }

        public RabbitMqConnectionConfigure GetRabbitMqConnectionConfigure()
        {
            return RabbitMqConnectionConfigure;
        }

        public List<RabbitMqPublishConfigure> GetRabbitMqPublishConfigures()
        {
            return RabbitMqPublishConfigures;
        }

        public RabbitMqPublishConfigure GetRabbitMqPublishConfigure(Type evenType)
        {
            RabbitMqPublishConfigure config = null;
            if (RabbitMqPublishConfigures != null &&
                RabbitMqPublishConfigures.Exists(p => p.EventType == evenType))
            {
                config = RabbitMqPublishConfigures.LastOrDefault(p => p.EventType == evenType);
            }
            return config;
        }

        public List<RabbitMqSubscribeConfigure> GetRabbitMqSubscribeConfigures()
        {
            return RabbitMqSubscribeConfigures;
        }

        public RabbitMqSubscribeConfigure GetRabbitMqSubscribeConfigure(Type evenType)
        {
            RabbitMqSubscribeConfigure config = null;
            if (RabbitMqSubscribeConfigures != null &&
                RabbitMqSubscribeConfigures.Exists(p => p.EventType == evenType))
            {
                config = RabbitMqSubscribeConfigures.LastOrDefault(p => p.EventType == evenType);
            }
            return config;
        }
    }
}
