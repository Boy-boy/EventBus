using Microsoft.Extensions.DependencyInjection;

namespace EventBus
{
    public class EventBusBuilder
    {
        public EventBusBuilder(IServiceCollection services)
        {
            this.Services = services;
        }

        public virtual IServiceCollection Services { get; }
    }
}
