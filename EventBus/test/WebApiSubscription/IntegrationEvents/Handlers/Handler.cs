using EventBus.Abstraction;
using PublishEvents;
using System;
using System.Threading.Tasks;

namespace WebApiSubscription.IntegrationEvents.Handlers
{
    public class UserEventHandler
        : IIntegrationEventHandler<UserEvent>
    {
        public async Task HandleAsync(UserEvent @event)
        {
            Console.WriteLine($"UserEventHandler，信息Id={@event.Name}");
            await Task.Delay(50);
        }

    }
}
