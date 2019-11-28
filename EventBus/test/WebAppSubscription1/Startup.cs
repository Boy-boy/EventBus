using Autofac;
using Autofac.Extensions.DependencyInjection;
using EventBus;
using EventBusRabbitMQ;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Threading.Tasks;

namespace WebAppSubscription1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<DbContext,SubscriptionDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DbSubscriptionContext"));
            });

            services.AddTransient<UserLocationUpdatedIntegrationEventHandler>();
            services.AddMvc(option => { option.EnableEndpointRouting = false; }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSingleton<IRabbitMqPersistentConnection>(sp =>
            {

                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMqPersistentConnection>>();

                var factory = new ConnectionFactory()
                {
                    HostName = Configuration["EventBusConnection"],
                    VirtualHost = Configuration["VirtualHost"],
                    DispatchConsumersAsync = true
                };

                if (!string.IsNullOrEmpty(Configuration["EventBusUserName"]))
                {
                    factory.UserName = Configuration["EventBusUserName"];
                }

                if (!string.IsNullOrEmpty(Configuration["EventBusPassword"]))
                {
                    factory.Password = Configuration["EventBusPassword"];
                }

                var retryCount = 5;
                if (!string.IsNullOrEmpty(Configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(Configuration["EventBusRetryCount"]);
                }

                return new DefaultRabbitMqPersistentConnection(factory, logger, retryCount);
            });


            var subscriptionClientName = Configuration["SubscriptionClientName"];
            services.AddSingleton<IEventBus, EventBusRabbitMq>(sp =>
            {
                var rabbitMqPersistentConnection = sp.GetRequiredService<IRabbitMqPersistentConnection>();
                var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMq>>();
                var eventBusSubscriptionManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                var retryCount = 5;
                if (!string.IsNullOrEmpty(Configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(Configuration["EventBusRetryCount"]);
                }
                return new EventBusRabbitMq(rabbitMqPersistentConnection, logger, iLifetimeScope, eventBusSubscriptionManager, subscriptionClientName, retryCount);
            });
            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            var container = new ContainerBuilder();
            container.Populate(services);
            return new AutofacServiceProvider(container.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            eventBus.Subscribe<UserLocationUpdatedIntegrationEvent, UserLocationUpdatedIntegrationEventHandler>();
        }
    }


    public class UserLocationUpdatedIntegrationEvent : IntegrationEvent
    {
        public UserLocationUpdatedIntegrationEvent()
        {
            EventTag = "publish";
        }

        public int Age { get; set; }
    }


    public class UserLocationUpdatedIntegrationEventHandler
        : IIntegrationEventHandler<UserLocationUpdatedIntegrationEvent>
    {
        private readonly SubscriptionDbContext _dbContext;

        public UserLocationUpdatedIntegrationEventHandler(DbContext dbContext)
        {
            _dbContext = dbContext as SubscriptionDbContext;
        }

        public async Task Handle(UserLocationUpdatedIntegrationEvent @event)
        {
            await Task.Yield();
            _dbContext.Students.Add(new Student()
            {
                Id = @event.Id,
                Age= @event.Age
            });
           await _dbContext.SaveChangesAsync();
        }

    }
}
