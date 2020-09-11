using EventBus;

namespace PublishEvents
{
    [EventName("UserEvent")]
    public class UserEvent : IntegrationEvent
    {
        public UserEvent(int age, string name)
        {
            Age = age;
            Name = name;
        }
        public int Age { get; set; }

        public string Name { get; set; }
    }
}
