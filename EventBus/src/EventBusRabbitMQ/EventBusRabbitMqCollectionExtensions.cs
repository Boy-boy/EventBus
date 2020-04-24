using EventBus;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Options;

namespace EventBusRabbitMQ
{
    public static class EventBusRabbitMqCollectionExtensions
    {
        public static void AddRabbitMq(this EventBusBuilder eventBusBuilder, Action<EventBusRabbitMqOption> configure)
        {
            eventBusBuilder.Services.AddSingleton<IRabbitMqPersistentConnection, DefaultRabbitMqPersistentConnection>();
            eventBusBuilder.Services.AddSingleton<IEventBus, EventBusRabbitMq>();
            if (configure == null) return;
            eventBusBuilder.Services.Configure(configure);
            var eventBusRabbitMqOption= eventBusBuilder.Services.BuildServiceProvider().GetService<IOptions<EventBusRabbitMqOption>>().Value;
            if (eventBusRabbitMqOption.RabbitMqSubscribeConfigures == null) return;
            foreach (var rabbitMqSubscribeConfigure in eventBusRabbitMqOption.RabbitMqSubscribeConfigures)
            {
                eventBusBuilder.AddBuildEventTag(rabbitMqSubscribeConfigure.EventType,
                    rabbitMqSubscribeConfigure.EventTag);
            }

        }
    }
}
