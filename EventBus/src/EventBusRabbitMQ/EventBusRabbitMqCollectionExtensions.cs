using EventBus;
using Microsoft.Extensions.DependencyInjection;
using System;
using EventBus.Abstraction;
using Microsoft.Extensions.Options;

namespace EventBusRabbitMQ
{
    public static class EventBusRabbitMqCollectionExtensions
    {
        public static EventBusBuilder AddRabbitMq(this EventBusBuilder eventBusBuilder, Action<EventBusRabbitMqOptions> configure)
        {
            eventBusBuilder.Services.AddSingleton<IRabbitMqPersistentConnection, DefaultRabbitMqPersistentConnection>();
            eventBusBuilder.Services.AddSingleton<IEventBus, EventBusRabbitMq>();
            if (configure == null) return eventBusBuilder;
            eventBusBuilder.Services.Configure(configure);
            var eventBusRabbitMqOption= eventBusBuilder.Services.BuildServiceProvider().GetRequiredService<IOptions<EventBusRabbitMqOptions>>().Value;
            if (eventBusRabbitMqOption.GetRabbitMqSubscribeConfigures() == null) return eventBusBuilder;
            foreach (var rabbitMqSubscribeConfigure in eventBusRabbitMqOption.GetRabbitMqSubscribeConfigures())
            {
                eventBusBuilder.AddEventTypeMapping(rabbitMqSubscribeConfigure.EventType,
                    rabbitMqSubscribeConfigure.EventTag);
            }
            return eventBusBuilder;
        }
    }
}
