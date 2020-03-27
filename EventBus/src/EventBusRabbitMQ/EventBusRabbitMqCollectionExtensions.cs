using EventBus;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EventBusRabbitMQ
{
    public static class EventBusRabbitMqCollectionExtensions
    {
        public static void AddRabbitMq(this EventBusBuilder eventBusBuilder, Action<EventBusRabbitMqOption> configure)
        {
            eventBusBuilder.Services.AddSingleton<IRabbitMqPersistentConnection, DefaultRabbitMqPersistentConnection>();
            eventBusBuilder.Services.AddSingleton<IEventBus, EventBusRabbitMq>();
            if (configure != null)
                eventBusBuilder.Services.Configure(configure);
        }
    }
}
