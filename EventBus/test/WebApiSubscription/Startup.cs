using EventBus.Abstraction;
using EventBusRabbitMQ;
using EventBusRabbitMQ.Configures;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PublishEvents;
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
                .AddRabbitMq(option =>
                {
                    var connectionConfigure = new RabbitMqConnectionConfigure();
                    Configuration.Bind(typeof(RabbitMqConnectionConfigure).Name, connectionConfigure);
                    option.ConfigRabbitMqConnectionConfigures(connectionConfigure)
                        .ConfigRabbitMqSubscribeConfigures(builder =>
                        {
                            builder.AddRabbitMqSubscribeConfigure(typeof(UserEvent),typeof(UserEvent).FullName,
                                    exchangeName: "WebAppPublishExchange");
                        });
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
            eventBus.Subscribe<UserEvent, UserEventHandler>();
        }
    }
}
