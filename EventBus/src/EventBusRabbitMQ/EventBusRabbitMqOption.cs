using EventBusRabbitMQ.Options;
using System.Collections.Generic;

namespace EventBusRabbitMQ
{
    public class EventBusRabbitMqOption
    {
        public EventBusRabbitMqOption()
        {
            RabbitMqConnectionOption=new RabbitMqConnectionOption();
            RabbitMqPublishOptions=new List<RabbitMqPublishOption>();
            RabbitMqSubscribeOptions=new List<RabbitMqSubscribeOption>();
        }
      
        public RabbitMqConnectionOption RabbitMqConnectionOption { get; set; }
        public List<RabbitMqPublishOption> RabbitMqPublishOptions { get; set; }
        public List<RabbitMqSubscribeOption> RabbitMqSubscribeOptions { get; set; }
    }
}
