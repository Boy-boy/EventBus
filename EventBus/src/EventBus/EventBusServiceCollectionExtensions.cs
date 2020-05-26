using EventBus;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EventBusServiceCollectionExtensions
    {
        public static EventBusBuilder AddEventBus(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            services.TryAddSingleton<IIntegrationEventSubscriptionBuildEventTagProvider, IntegrationEventSubscriptionBuildEventTagProvider>();
            services.TryAddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
            var builder = new EventBusBuilder(services);
            return builder;
        }
        public static EventBusBuilder AddEventHandler<TEvent, THandler>(this EventBusBuilder eventBusBuilder)
            where THandler : class, IIntegrationEventHandler<TEvent>
            where TEvent : IntegrationEvent
        {
            var eventHandlerType = typeof(IIntegrationEventHandler<>).MakeGenericType(typeof(TEvent));
            eventBusBuilder.Services.AddTransient(eventHandlerType, typeof(THandler));
            return eventBusBuilder;
        }
        public static EventBusBuilder AddEventHandlers<TEvent, THandler0, THandler1>(this EventBusBuilder eventBusBuilder)
            where THandler0 : class, IIntegrationEventHandler<TEvent>
            where THandler1 : class, IIntegrationEventHandler<TEvent>
            where TEvent : IntegrationEvent
        {
            var eventHandlerType = typeof(IIntegrationEventHandler<>).MakeGenericType(typeof(TEvent));
            eventBusBuilder.Services.AddTransient(eventHandlerType, typeof(THandler0));
            eventBusBuilder.Services.AddTransient(eventHandlerType, typeof(THandler1));
            return eventBusBuilder;
        }
        public static EventBusBuilder AddEventHandlers<TEvent, THandler0, THandler1, THandler2>(this EventBusBuilder eventBusBuilder)
            where THandler0 : class, IIntegrationEventHandler<TEvent>
            where THandler1 : class, IIntegrationEventHandler<TEvent>
            where THandler2 : class, IIntegrationEventHandler<TEvent>
            where TEvent : IntegrationEvent
        {
            var eventHandlerType = typeof(IIntegrationEventHandler<>).MakeGenericType(typeof(TEvent));
            eventBusBuilder.Services.AddTransient(eventHandlerType, typeof(THandler0));
            eventBusBuilder.Services.AddTransient(eventHandlerType, typeof(THandler1));
            eventBusBuilder.Services.AddTransient(eventHandlerType, typeof(THandler2));
            return eventBusBuilder;
        }
        public static EventBusBuilder AddEventHandlers<TEvent, THandler0, THandler1, THandler2, THandler3>(this EventBusBuilder eventBusBuilder)
            where THandler0 : class, IIntegrationEventHandler<TEvent>
            where THandler1 : class, IIntegrationEventHandler<TEvent>
            where THandler2 : class, IIntegrationEventHandler<TEvent>
            where THandler3 : class, IIntegrationEventHandler<TEvent>
            where TEvent : IntegrationEvent
        {
            var eventHandlerType = typeof(IIntegrationEventHandler<>).MakeGenericType(typeof(TEvent));
            eventBusBuilder.Services.AddTransient(eventHandlerType, typeof(THandler0));
            eventBusBuilder.Services.AddTransient(eventHandlerType, typeof(THandler1));
            eventBusBuilder.Services.AddTransient(eventHandlerType, typeof(THandler2));
            eventBusBuilder.Services.AddTransient(eventHandlerType, typeof(THandler3));
            return eventBusBuilder;
        }
    }
}
