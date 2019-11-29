using System.Collections.Generic;
using EventBus.Provider;
using EventBusRabbitMQ.Provider;

namespace EventBusRabbitMQ
{
    public class EventBusRabbitMqOption
    {
        public EventBusRabbitMqOption()
        {
            SubscriptionsIntegrationEventOption=new SubscriptionsIntegrationEventOption();
            RabbitMqConnectionOption=new RabbitMqConnectionOption();
            RabbitMqPublishOptions=new List<RabbitMqPublishOption>();
            RabbitMqSubscribeOptions=new List<RabbitMqSubscribeOption>();
        }
        public SubscriptionsIntegrationEventOption SubscriptionsIntegrationEventOption { get; set; }
        public RabbitMqConnectionOption RabbitMqConnectionOption { get; set; }
        public List<RabbitMqPublishOption> RabbitMqPublishOptions { get; set; }
        public List<RabbitMqSubscribeOption> RabbitMqSubscribeOptions { get; set; }
    }
}
