using EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiSubscription.IntegrationEvents.Events
{
    public class UserLocationUpdatedIntegrationEvent : IntegrationEvent
    {
        public int Age { get; set; }
    }

    public class UserEvent : IntegrationEvent
    {
        public int Age { get; set; }

        public string Name { get; set; }
    }
}
