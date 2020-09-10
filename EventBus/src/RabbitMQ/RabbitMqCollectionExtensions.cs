using Microsoft.Extensions.DependencyInjection;
using System;

namespace RabbitMQ
{
    public static class RabbitMqCollectionExtensions
    {
        public static IServiceCollection AddRabbitMq(this IServiceCollection services, Action<RabbitMqOptions> configureOptions)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            services.AddSingleton<IRabbitMqPersistentConnection, DefaultRabbitMqPersistentConnection>();
            services.AddSingleton<IRabbitMqMessageConsumer, DefaultRabbitMqMessageConsumer>();
            if (configureOptions != null)
                services.Configure(configureOptions);
            return services;
        }
    }
}
