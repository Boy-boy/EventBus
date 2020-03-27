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

        /// <summary>The services being configured.</summary>
        public virtual IServiceCollection Services { get; }

        public virtual EventBusBuilder AddEventMappingTagHelper(Type evenType, string eventTag)
        {
            Services.Configure<EventBusOptions>(option => option.AddEventMappingTag(builder =>
            {
                builder.EventType = evenType;
                builder.EventTag = eventTag;
            }));
            return this;
        }
    }
}
