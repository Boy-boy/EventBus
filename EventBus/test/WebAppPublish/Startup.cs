using Autofac;
using Autofac.Extensions.DependencyInjection;
using EventBusRabbitMQ;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System;
using EventBusRabbitMQ.Provider;
using WebAppPublish.Event;

namespace WebAppPublish
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

            services.AddEventBusRabbitMq(option =>
            {
                option.ConnectionFactory = new ConnectionFactory()
                {
                    HostName = "127.0.0.1",
                    VirtualHost = "/",
                    DispatchConsumersAsync = true,
                    UserName = "guest",
                    Password = "guest"
                };
            });
            services.AddRabbitMqPublishProvider(options =>
            {
                options.Add(new RabbitMqPublishOption(typeof(UserLocationUpdatedIntegrationEvent), "UserLocationUpdatedIntegrationEvent"));
            });
            var container = new ContainerBuilder();
            container.Populate(services);
            return new AutofacServiceProvider(container.Build());
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
