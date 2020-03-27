using System;
using EventBus.Provider;
using Microsoft.Extensions.DependencyInjection;

namespace EventBus
{
    public class EventBusBuilder
    {
        public EventBusBuilder(IServiceCollection services)
        {
            this.Services = services;
        }

        /// <summary>The services being configured.</summary>
        public virtual IServiceCollection Services { get; }


        public virtual EventBusBuilder AddSubscriptionEventMappingTagOption(Action<IntegrationEventSubscriptionEventMappingTagOption> option)
        {
            this.Services.Configure(option);
            return this;
        }
    }
}
