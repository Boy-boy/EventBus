using System;
using System.Collections.Generic;

namespace EventBus.Provider
{
    public class IntegrationEventSubscriptionEventMappingTagBuilder
    {
        private readonly List<IntegrationEventSubscriptionEventMappingTag> _eventsMappingTags = new List<IntegrationEventSubscriptionEventMappingTag>();
        public IntegrationEventSubscriptionEventMappingTagBuilder() { }

        public void AddEventMappingTag(Type evenType,string eventTag)
        {
            _eventsMappingTags.Add(new IntegrationEventSubscriptionEventMappingTag(evenType,eventTag));
        }
        public void AddEventMappingTag(IntegrationEventSubscriptionEventMappingTag eventMappingTag)
        {
            if(eventMappingTag==null)
                throw new ArgumentNullException(nameof(eventMappingTag));
            _eventsMappingTags.Add(eventMappingTag);
        }


        public List<IntegrationEventSubscriptionEventMappingTag> Build()
        {
            return _eventsMappingTags??new List<IntegrationEventSubscriptionEventMappingTag>();
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
