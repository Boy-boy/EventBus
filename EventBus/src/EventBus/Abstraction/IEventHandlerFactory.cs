using System;

namespace EventBus.Abstraction
{
    public interface IEventHandlerFactory
    {
        IIntegrationEventHandler GetHandler(Type handlerType);
    }
}
