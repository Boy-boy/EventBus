using System;

namespace EventBus
{
    public class IntegrationEventSubscriptionBuildEventTagBuilder
    {

        public IntegrationEventSubscriptionBuildEventTagBuilder() { }

        public Type EventType { get; set; }
        public string EventTag { get; set; }

        public IntegrationEventSubscriptionBuildEventTag Build()
        {
            return new IntegrationEventSubscriptionBuildEventTag(EventType, EventTag);
        }
    }

    public class IntegrationEventSubscriptionBuildEventTag
    {
        public IntegrationEventSubscriptionBuildEventTag(Type eventType, string eventTag)
        {
            EventType = eventType ?? throw new ArgumentNullException(nameof(eventType));
            EventTag = eventTag;
        }
        public Type EventType { get; set; }
        public string EventTag { get; set; }
    }
}
