using EventBus.Abstraction;
using System;

namespace EventBus
{
    public class EventHandlerWrapper
    {
        public Type EventHandlerType { get; }

        public IIntegrationEventHandler EventHandler { get; }

        public EventHandlerWrapper(Type eventHandlerType, IIntegrationEventHandler eventHandler)
        {
            EventHandlerType = eventHandlerType;
            EventHandler = eventHandler;
        }
    }
}
