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
            services.AddTransient<UserLocationUpdatedIntegrationEventHandler>();
            services.AddTransient<UserLocationUpdatedIntegrationEventHandler1>();
            services.AddTransient<UserEventHandler>();
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
                                    "WebAppPublishExchange")
                                .AddRabbitMqSubscribeConfigure(typeof(UserEvent),
                                    exchangeName: "WebAppPublishExchange");
                        });
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,IHostApplicationLifetime hostApplicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            hostApplicationLifetime.ApplicationStopping.Register(() =>
            {
                eventBus.Dispose();
            });
            eventBus.Subscribe<UserLocationUpdatedIntegrationEvent, UserLocationUpdatedIntegrationEventHandler>();
            eventBus.Subscribe<UserLocationUpdatedIntegrationEvent, UserLocationUpdatedIntegrationEventHandler1>();
            eventBus.Subscribe<UserEvent, UserEventHandler>();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
