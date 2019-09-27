using EventBus;

namespace WebAppPublish.Event
{
    public class UserLocationUpdatedIntegrationEvent : IntegrationEvent
    {
        public UserLocationUpdatedIntegrationEvent(int age)
        {
            Age = age;
        }
        public int Age { get; set; }
    }
}
