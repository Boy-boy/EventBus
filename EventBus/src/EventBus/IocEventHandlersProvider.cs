using EventBus.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EventBus
{
    public class IocEventHandlersProvider : IEventHandlersProvider
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public IocEventHandlersProvider(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        public EventHandlerWrapper GetHandler(Type handlerType)
        {
            var scope = _serviceScopeFactory.CreateScope();
            return new EventHandlerWrapper(
                handlerType,
                (IIntegrationEventHandler)scope.ServiceProvider.GetRequiredService(handlerType)
            );
        }
    }
}
