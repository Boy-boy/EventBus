using EventBus.Abstraction;
using System.Collections.Generic;

namespace EventBus
{
    public class EventBusOptions
    {
        public IList<IIntegrationEventHandler> SubscriptionHandlers { get; }

        public EventBusOptions()
        {
            SubscriptionHandlers = new List<IIntegrationEventHandler>();
        }
    }
}
