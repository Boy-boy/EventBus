using EventBus.Abstraction;
using EventBus.RabbitMQ;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PublishEvents;
using RabbitMQ;
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
            services.AddRabbitMq(option =>
            {
                option.Connection.HostName = "127.0.0.1";
                option.Connection.UserName = "guest";
                option.Connection.Password = "guest";
                option.Connection.Port = -1;
                option.Connection.VirtualHost = "/";

            });
            services.AddEventBus()
                .AddEventHandler<UserEvent, UserEventHandler>()
                .AddRabbitMq(option =>
                {
                    option.SetExchangeAndQueue("WebAppPublishExchange", "event_bus_rabbitmq_default_queue");
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
