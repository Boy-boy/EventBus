using System;

namespace EventBus
{
    public class IntegrationEventSubscriptionEventMappingTagBuilder
    {

        public IntegrationEventSubscriptionEventMappingTagBuilder() { }

        public Type EventType { get; set; }
        public string EventTag { get; set; }

        public IntegrationEventSubscriptionEventMappingTag Build()
        {
            return new IntegrationEventSubscriptionEventMappingTag(EventType, EventTag);
        }
    }

    public class IntegrationEventSubscriptionEventMappingTag
    {
        public IntegrationEventSubscriptionEventMappingTag(Type eventType, string eventTag)
        {
            EventType = eventType ?? throw new ArgumentNullException(nameof(eventType));
            EventTag = eventTag ?? throw new ArgumentNullException(nameof(eventTag));
        }
        public Type EventType { get; set; }
        public string EventTag { get; set; }
    }
}
