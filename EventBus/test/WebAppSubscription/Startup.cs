using Autofac;
using Autofac.Extensions.DependencyInjection;
using EventBus;
using EventBusRabbitMQ;
using EventBusRabbitMQ.Configures;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
                    var connectionConfigure = new RabbitMqConnectionConfigure();
                    Configuration.Bind(typeof(RabbitMqConnectionConfigure).Name, connectionConfigure);
                    configure.ConfigRabbitMqConnectionConfigures(connectionConfigure)
                        .ConfigRabbitMqSubscribeConfigures(new List<RabbitMqSubscribeConfigure>()
                        {
                            new RabbitMqSubscribeConfigure(typeof(UserLocationUpdatedIntegrationEvent), 
                                "UserLocationUpdatedIntegrationEventExchange",eventTag:typeof(UserLocationUpdatedIntegrationEvent).Name)
                        });
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
