using EventBus;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Options;

namespace EventBusRabbitMQ
{
    public static class EventBusRabbitMqCollectionExtensions
    {
        public static void AddRabbitMq(this EventBusBuilder eventBusBuilder, Action<EventBusRabbitMqOptions> configure)
        {
            eventBusBuilder.Services.AddSingleton<IRabbitMqPersistentConnection, DefaultRabbitMqPersistentConnection>();
            eventBusBuilder.Services.AddSingleton<IEventBus, EventBusRabbitMq>();
            if (configure == null) return;
            eventBusBuilder.Services.Configure(configure);
            var eventBusRabbitMqOption= eventBusBuilder.Services.BuildServiceProvider().GetRequiredService<IOptions<EventBusRabbitMqOptions>>().Value;
            if (eventBusRabbitMqOption.GetRabbitMqSubscribeConfigures() == null) return;
            foreach (var rabbitMqSubscribeConfigure in eventBusRabbitMqOption.GetRabbitMqSubscribeConfigures())
            {
                eventBusBuilder.AddBuildEventTag(rabbitMqSubscribeConfigure.EventType,
                    rabbitMqSubscribeConfigure.EventTag);
            }

        }
    }
}
