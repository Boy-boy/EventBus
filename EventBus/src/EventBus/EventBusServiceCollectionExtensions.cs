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
            services.TryAddSingleton<IIntegrationEventSubscriptionBuildEventTagProvider, IntegrationEventSubscriptionBuildEventTagProvider>();
            services.TryAddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
            if (configureOptions != null)
                services.Configure(configureOptions);
            var builder = new EventBusBuilder(services);
            return builder;
        }
    }
}
