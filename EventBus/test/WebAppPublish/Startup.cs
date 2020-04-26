using Autofac;
using Autofac.Extensions.DependencyInjection;
using EventBusRabbitMQ;
using EventBusRabbitMQ.Configures;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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

            services.AddEventBus()
                .AddRabbitMq(configure =>
                {
                    var connectionConfigure = new RabbitMqConnectionConfigure();
                    Configuration.Bind(typeof(RabbitMqConnectionConfigure).Name, connectionConfigure);
                    configure.ConfigRabbitMqConnectionConfigures(connectionConfigure)
                        .ConfigRabbitMqPublishConfigures(builder=>
                        {
                            builder.AddRabbitMqPublishConfigure(typeof(UserLocationUpdatedIntegrationEvent),
                                "UserLocationUpdatedIntegrationEventExchange");
                        });
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
