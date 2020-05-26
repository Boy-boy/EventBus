using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace EventBus
{
    public interface IEventHandlerFactory
    {
        ICollection<IIntegrationEventHandler> GetHandlers(Type eventType);
    }

    public class EventHandlerFactory : IEventHandlerFactory
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public EventHandlerFactory(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        public ICollection<IIntegrationEventHandler> GetHandlers(Type eventType)
        {
            var eventHandlerType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
            return _serviceScopeFactory
                .CreateScope()
                .ServiceProvider
                .GetServices(eventHandlerType)
                .Cast<IIntegrationEventHandler>()
                .ToArray();
        }
    }
}
