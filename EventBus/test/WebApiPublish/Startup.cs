using EventBus.RabbitMQ;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ;

namespace WebApiPublish
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

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
                .AddRabbitMq(option =>
                {
                    option.SetExchangeAndQueue("WebAppPublishExchange", "event_bus_rabbitmq_default_queue");
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
