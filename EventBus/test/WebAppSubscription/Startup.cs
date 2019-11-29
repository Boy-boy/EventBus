using Autofac;
using Autofac.Extensions.DependencyInjection;
using EventBus;
using EventBusRabbitMQ;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using EventBus.Provider;
using EventBusRabbitMQ.Provider;
using WebAppSubscription.IntegrationEvents.Events;
using WebAppSubscription.IntegrationEvents.Handlers;

namespace WebAppSubscription
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(option => { option.EnableEndpointRouting = false; });
            services.AddTransient<UserLocationUpdatedIntegrationEventHandler>();
            services.AddTransient<UserLocationUpdatedIntegrationEventHandler1>();

            services.AddEventBusRabbitMq(option =>
            {
                option.RabbitMqConnectionOption = new RabbitMqConnectionOption()
                {
                    ConnectionFactory = new ConnectionFactory()
                    {
                        HostName = "127.0.0.1",
                        VirtualHost = "/",
                        DispatchConsumersAsync = true,
                        UserName = "guest",
                        Password = "guest"
                    }
                };
                option.RabbitMqSubscribeOptions = new List<RabbitMqSubscribeOption>
                {
                    new RabbitMqSubscribeOption(typeof(UserLocationUpdatedIntegrationEvent),
                        "UserLocationUpdatedIntegrationEvent", "UserLocationUpdatedIntegrationEvent")
                };
                option.SubscriptionsIntegrationEventOption = new SubscriptionsIntegrationEventOption()
                    .AddSubscriptionsIntegrationEventOption(typeof(UserLocationUpdatedIntegrationEvent), "publish");
               ;
            });
            var container = new ContainerBuilder();
            container.Populate(services);
            return new AutofacServiceProvider(container.Build());
        }

        public void Configure(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            eventBus.Subscribe<UserLocationUpdatedIntegrationEvent, UserLocationUpdatedIntegrationEventHandler>();
            eventBus.Subscribe<UserLocationUpdatedIntegrationEvent, UserLocationUpdatedIntegrationEventHandler1>();
            app.UseMvc();
        }
    }
}
