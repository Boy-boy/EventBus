using System;
using System.Collections.Generic;

namespace EventBus
{
    public class EventBusOptions
    {
        public Dictionary<string, Type> EventTypeMapping { get; } = new Dictionary<string, Type>();

        public void AddEventTypeMapping(Type evenType, string eventTag)
        {
            if (evenType == null)
                throw new ArgumentNullException(nameof(evenType));
            if (string.IsNullOrEmpty(eventTag))
                throw new ArgumentNullException(nameof(eventTag));
            if (EventTypeMapping.ContainsKey(eventTag))
            {
                if (EventTypeMapping[eventTag] != evenType)
                {
                    throw new ArgumentException(
                        $"Event Tag {eventTag} already exists,please make sure the event key is unique");
                }
            }
            else
            {
                EventTypeMapping.Add(eventTag, evenType);
            }

        }
    }
}
