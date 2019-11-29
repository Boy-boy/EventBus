﻿using System;
using System.Collections.Generic;
using EventBus;
using EventBus.Provider;
using EventBusRabbitMQ.Provider;
using Microsoft.Extensions.DependencyInjection;

namespace EventBusRabbitMQ
{
    public static class EventBusRabbitMqCollectionExtensions
    {
        public static void AddEventBusRabbitMq(this IServiceCollection services, Action<EventBusRabbitMqOption> fun)
        {
            services.AddSingleton<IRabbitMqPersistentConnection, DefaultRabbitMqPersistentConnection>();
            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
            services.AddSingleton<IEventBus, EventBusRabbitMq>();

            var eventBusRabbitMqOption = new EventBusRabbitMqOption(); 
            fun.Invoke(eventBusRabbitMqOption);
            services.AddRabbitMqPublishProvider(options =>
            {
                options.AddRange(eventBusRabbitMqOption.RabbitMqPublishOptions) ;
            });
            services.AddRabbitMqSubscribeProvider(options =>
            {
                options.AddRange(eventBusRabbitMqOption.RabbitMqSubscribeOptions);
            });
            services.AddRabbitMqConnectionProvider(option =>
            {
                option.ConnectionFactory = eventBusRabbitMqOption.RabbitMqConnectionOption.ConnectionFactory;
            });
            services.AddSubscriptionsIntegrationEventOptionProvider(option =>
            {
                eventBusRabbitMqOption.SubscriptionsIntegrationEventOption.SubscriptionsIntegrationEvents.ForEach(p =>
                {
                    option.AddSubscriptionsIntegrationEventOption(p.EventType,p.EventTag); 
                });
            });
        }
        public static void AddRabbitMqPublishProvider(this IServiceCollection services, Action<List<RabbitMqPublishOption>> options)
        {
            services.Configure(options);
            services.AddSingleton<RabbitMqPublishProvider>();
        }
        public static void AddRabbitMqSubscribeProvider(this IServiceCollection services, Action<List<RabbitMqSubscribeOption>> options)
        {
            services.Configure(options);
            services.AddSingleton<RabbitMqSubscribeProvider>();
        }
        public static void AddRabbitMqConnectionProvider(this IServiceCollection services, Action<RabbitMqConnectionOption> option)
        {
            services.Configure(option);
            services.AddSingleton<RabbitMqConnectionProvider>();
        }
        public static void AddSubscriptionsIntegrationEventOptionProvider(this IServiceCollection services, Action<SubscriptionsIntegrationEventOption> option)
        {
            services.Configure(option);
            services.AddSingleton<SubscriptionsIntegrationEventOptionProvider>();
        }
    }
}
