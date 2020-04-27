using EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiPublish.Event
{
    public class UserLocationUpdatedIntegrationEvent : IntegrationEvent
    {
        public UserLocationUpdatedIntegrationEvent(int age, string play)
        {
            Age = age;
            Play = play;
            EventTag = typeof(UserLocationUpdatedIntegrationEvent).Name;
        }
        public int Age { get; set; }

        public string Play { get; set; }
    }

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
