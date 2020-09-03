using Microsoft.Extensions.DependencyInjection;
using System;

namespace EventBus
{
    public class EventBusBuilder
    {
        public EventBusBuilder(IServiceCollection services)
        {
            this.Services = services;
        }

        public virtual IServiceCollection Services { get; }

        public virtual EventBusBuilder AddEventTypeMapping(Type evenType, string eventTag)
        {
            Services.Configure<EventBusOptions>(option => option.AddEventTypeMapping(evenType, eventTag));
            return this;
        }
    }
}
