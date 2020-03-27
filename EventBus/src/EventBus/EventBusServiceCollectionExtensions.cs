using EventBus;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using EventBus.Provider;

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
            services.TryAddScoped<IIntegrationEventSubscriptionsEventMappingTagProvider, IntegrationEventSubscriptionsEventMappingTagProvider>();
            services.AddDefaultEventBusSubscriptionsManager();
            var builder = new EventBusBuilder(services);
            return builder;
        }
        public static IServiceCollection AddDefaultEventBusSubscriptionsManager(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            services.TryAddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
            return services;
        }
    }
}
