using EventBus;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EventBusServiceCollectionExtensions
    {
        public static EventBusBuilder AddEventBus(this IServiceCollection services,Action<EventBusOptions> configureOptions=null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (configureOptions != null)
                services.Configure(configureOptions);
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
            services.TryAddSingleton<IIntegrationEventSubscriptionEventMappingTagProvider, IntegrationEventSubscriptionEventMappingTagProvider>();
            services.TryAddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
            return services;
        }
    }
}
