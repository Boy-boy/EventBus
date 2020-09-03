using System;
using System.Collections.Generic;
using EventBus.Abstraction;
using Microsoft.Extensions.Options;

namespace EventBus
{
    public class EventTypeProvider : IEventTypeProvider
    {
        private readonly Dictionary<string, Type> _eventTypes;

        public EventTypeProvider(IOptions<EventBusOptions> options)
        {
            _eventTypes = options.Value.EventTypeMapping;
        }

        public Type TryGetEventType(string eventKey)
        {
            if (_eventTypes.TryGetValue(eventKey, out var eventType) && eventType != null)
                return eventType;
            return null;
        }

        public bool HasEventType(string eventKey)
        {
            return _eventTypes.ContainsKey(eventKey);
        }
    }
}
