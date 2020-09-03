using EventBus.Abstraction;
using System;
using System.Collections.Generic;

namespace EventBus
{
    public class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionsManager
    {
        private readonly IEventHandlersProvider _eventHandlersProvider;
        private readonly IEventTypeProvider _eventTypeProvider;

        public InMemoryEventBusSubscriptionsManager(
            IEventHandlersProvider eventHandlersProvider,
            IEventTypeProvider eventTypeProvider)
        {
            _eventHandlersProvider = eventHandlersProvider;
            _eventTypeProvider = eventTypeProvider;
        }
        public ICollection<IIntegrationEventHandler> GetHandlers(string eventKey)
        {
            var eventType = TryGetEventTypeByEventKey(eventKey);
            return _eventHandlersProvider.GetHandlers(eventType) ?? new List<IIntegrationEventHandler>();
        }

        public Type TryGetEventTypeByEventKey(string eventKey)
        {
            return _eventTypeProvider.TryGetEventType(eventKey);
        }

        public bool HasEventTypeByEventKey(string eventKey)
        {
            return _eventTypeProvider.HasEventType(eventKey);
        }
    }
}
