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

        public virtual EventBusBuilder AddBuildEventTag(Type evenType, string eventTag)
        {
            Services.Configure<EventBusOptions>(option => option.AddBuildEventTag(builder =>
            {
                builder.EventType = evenType;
                builder.EventTag = eventTag;
            }));
            return this;
        }
    }
}
