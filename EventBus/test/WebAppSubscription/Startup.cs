using Autofac;
using Autofac.Extensions.DependencyInjection;
using EventBus;
using EventBusRabbitMQ;
using EventBusRabbitMQ.Configures;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
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
                .AddRabbitMq(option =>
                {
                    var connectionConfigure = new RabbitMqConnectionConfigure();
                    Configuration.Bind(typeof(RabbitMqConnectionConfigure).Name, connectionConfigure);
                    option.ConfigRabbitMqConnectionConfigures(connectionConfigure)
                        .ConfigRabbitMqSubscribeConfigures(builder =>
                        {
                            builder.AddRabbitMqSubscribeConfigure(typeof(UserLocationUpdatedIntegrationEvent),
                                typeof(UserLocationUpdatedIntegrationEvent).Name,
                                "UserLocationUpdatedIntegrationEventExchange");
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
            eventBus.Subscribe<UserLocationUpdatedIntegrationEvent, UserLocationUpdatedIntegrationEventHandler1>();
            app.UseMvc();
        }
    }
}
