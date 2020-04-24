﻿using Autofac;
using Autofac.Extensions.DependencyInjection;
using EventBus;
using EventBusRabbitMQ;
using EventBusRabbitMQ.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
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

            services.AddEventBus()
                .AddRabbitMq(configure =>
                {
                    var connectionFactory = new ConnectionFactory();
                    Configuration.Bind(typeof(ConnectionFactory).Name, connectionFactory);
                    configure.RabbitMqConnectionOption = new RabbitMqConnectionOption()
                    {
                        ConnectionFactory = connectionFactory
                    };
                    configure.RabbitMqSubscribeOptions = new List<RabbitMqSubscribeOption>
                    {
                        new RabbitMqSubscribeOption(typeof(UserLocationUpdatedIntegrationEvent),
                            "UserLocationUpdatedIntegrationEventExchange", typeof(UserLocationUpdatedIntegrationEvent).Assembly.GetName().Name)
                    };
                });
            var container = new ContainerBuilder();
            container.Populate(services);
            return new AutofacServiceProvider(container.Build());
        }

        public void Configure(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            eventBus.Subscribe<UserLocationUpdatedIntegrationEvent, UserLocationUpdatedIntegrationEventHandler>();
            app.UseMvc();
        }
    }
}
