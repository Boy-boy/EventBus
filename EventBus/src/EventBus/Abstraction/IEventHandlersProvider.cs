using System;

namespace EventBus.Abstraction
{
    public interface IEventHandlersProvider
    {
        EventHandlerWrapper GetHandler(Type handlerType);
    }
}
