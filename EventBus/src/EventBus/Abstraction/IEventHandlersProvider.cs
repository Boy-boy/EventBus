using System;
using System.Collections.Generic;

namespace EventBus.Abstraction
{
    public interface IEventHandlersProvider
    {
        ICollection<IIntegrationEventHandler> GetHandlers(Type eventType);
    }
}
