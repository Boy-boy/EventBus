using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using EventBus.Abstraction;

namespace EventBus
{
    public class EventHandlersProvider : IEventHandlersProvider
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public EventHandlersProvider(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public ICollection<IIntegrationEventHandler> GetHandlers(Type eventType)
        {
            if (eventType == null)
                throw new ArgumentNullException(nameof(eventType));
            var eventHandlerType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
            return _serviceScopeFactory
                .CreateScope()
                .ServiceProvider
                .GetServices(eventHandlerType)
                ?.Cast<IIntegrationEventHandler>()
                .ToList();
        }
    }
}
