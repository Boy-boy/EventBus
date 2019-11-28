using EventBus;

namespace WebAppPublish.Event
{
    public class UserLocationUpdatedIntegrationEvent : IntegrationEvent
    {
        public UserLocationUpdatedIntegrationEvent(int age)
        {
            Age = age;
            EventTag = "publish";
        }
        public int Age { get; set; }
    }
}
