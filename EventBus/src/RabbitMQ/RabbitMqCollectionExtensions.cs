﻿using Microsoft.Extensions.DependencyInjection;
using System;

namespace RabbitMQ
{
    public static class RabbitMqCollectionExtensions
    {
        public static IServiceCollection AddRabbitMq(this IServiceCollection services, Action<RabbitMqOptions> configureOptions = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            services.AddSingleton<IRabbitMqPersistentConnection, DefaultRabbitMqPersistentConnection>();
            services.AddSingleton<IRabbitMqMessageConsumerFactory, DefaultRabbitMqMessageConsumerFactory>();
            if (configureOptions != null)
                services.Configure(configureOptions);
            return services;
        }
    }
}
