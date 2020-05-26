using System;
using System.Linq;
using EventBus;
using EventBusRabbitMQ;
using EventBusRabbitMQ.Configures;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApiSubscription.IntegrationEvents.Events;
using WebApiSubscription.IntegrationEvents.Handlers;

namespace WebApiSubscription
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEventBus()
                .AddEventHandler<UserEvent, UserEventHandler>()
                .AddEventHandlers<UserLocationUpdatedIntegrationEvent, UserLocationUpdatedIntegrationEventHandler, UserLocationUpdatedIntegrationEventHandler1>()
                .AddRabbitMq(option =>
                {
                    var connectionConfigure = new RabbitMqConnectionConfigure();
                    Configuration.Bind(typeof(RabbitMqConnectionConfigure).Name, connectionConfigure);
                    option.ConfigRabbitMqConnectionConfigures(connectionConfigure)
                        .ConfigRabbitMqSubscribeConfigures(builder =>
                        {
                            builder.AddRabbitMqSubscribeConfigure(typeof(UserLocationUpdatedIntegrationEvent),
                                    typeof(UserLocationUpdatedIntegrationEvent).Name,
                                    "WebAppPublishExchange")
                                .AddRabbitMqSubscribeConfigure(typeof(UserEvent),
                                    exchangeName: "WebAppPublishExchange");
                        });
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime hostApplicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            ConfigureEventBus(app);
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            eventBus.Subscribe<UserLocationUpdatedIntegrationEvent, UserLocationUpdatedIntegrationEventHandler>();
            eventBus.Subscribe<UserLocationUpdatedIntegrationEvent, UserLocationUpdatedIntegrationEventHandler1>();
            eventBus.Subscribe<UserEvent, UserEventHandler>();
        }
    }
}
