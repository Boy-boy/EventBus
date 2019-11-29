using System;
using System.Collections.Generic;

namespace EventBus.Provider
{
    public class SubscriptionsIntegrationEventOption
    {
        public List<SubscriptionsIntegrationEvent> SubscriptionsIntegrationEvents;

        public SubscriptionsIntegrationEventOption()
        {
            SubscriptionsIntegrationEvents=new List<SubscriptionsIntegrationEvent>();
        }
        public SubscriptionsIntegrationEventOption AddSubscriptionsIntegrationEventOption(SubscriptionsIntegrationEvent subscriptionsIntegration)
        {
            SubscriptionsIntegrationEvents.Add(subscriptionsIntegration);
            return this;
        }

        public SubscriptionsIntegrationEventOption AddSubscriptionsIntegrationEventOption(Type eventType, string eventTag)
        {
            SubscriptionsIntegrationEvents.Add(new SubscriptionsIntegrationEvent(eventType, eventTag));
            return this;
        }
    }

    public class SubscriptionsIntegrationEvent
    {
        public SubscriptionsIntegrationEvent(Type eventType, string eventTag)
        {
            EventType = eventType;
            EventTag = eventTag;
        }
        public Type EventType { get; set; }
        public string EventTag { get; set; }
    }
}
