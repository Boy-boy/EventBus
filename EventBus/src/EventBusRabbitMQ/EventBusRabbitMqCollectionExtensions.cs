using EventBus.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EventBus.RabbitMQ
{
    public static class EventBusRabbitMqCollectionExtensions
    {
        public static EventBusBuilder AddRabbitMq(this EventBusBuilder eventBusBuilder, Action<EventBusRabbitMqOptions> configure)
        {
            eventBusBuilder.Services.AddSingleton<IRabbitMqPersistentConnection, DefaultRabbitMqPersistentConnection>();
            eventBusBuilder.Services.AddSingleton<IEventBus, EventBusRabbitMq>();
            if (configure == null) return eventBusBuilder;
            eventBusBuilder.Services.Configure(configure);
            return eventBusBuilder;
        }
    }
}
