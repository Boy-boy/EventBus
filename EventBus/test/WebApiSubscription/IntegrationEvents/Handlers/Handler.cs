using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBus;
using WebApiSubscription.IntegrationEvents.Events;

namespace WebApiSubscription.IntegrationEvents.Handlers
{
    public class UserLocationUpdatedIntegrationEventHandler
        : IIntegrationEventHandler<UserLocationUpdatedIntegrationEvent>
    {


        public async Task Handle(UserLocationUpdatedIntegrationEvent @event)
        {
            Console.WriteLine($"UserLocationUpdatedIntegrationEventHandler，信息Id={@event.Id}");
            await Task.Delay(50);
        }

    }
    public class UserLocationUpdatedIntegrationEventHandler1
        : IIntegrationEventHandler<UserLocationUpdatedIntegrationEvent>
    {
        public async Task Handle(UserLocationUpdatedIntegrationEvent @event)
        {
            Console.WriteLine($"UserLocationUpdatedIntegrationEventHandler1，信息Id={@event.Id}");
            await Task.Delay(50);
        }
    }

    public class UserEventHandler
        : IIntegrationEventHandler<UserEvent>
    {
        public async Task Handle(UserEvent @event)
        {
            Console.WriteLine($"UserEventHandler，信息Id={@event.Name}");
            await Task.Delay(50);
        }

    }
}
