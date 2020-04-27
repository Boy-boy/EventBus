using EventBus;
using System.Threading.Tasks;
using WebAppSubscription.IntegrationEvents.Events;

namespace WebAppSubscription.IntegrationEvents.Handlers
{
    public class UserLocationUpdatedIntegrationEventHandler
          : IIntegrationEventHandler<UserLocationUpdatedIntegrationEvent>
    {
        

        public async Task Handle(UserLocationUpdatedIntegrationEvent @event)
        {
            await Task.Delay(0);
        }

    }
    public class UserLocationUpdatedIntegrationEventHandler1
        : IIntegrationEventHandler<UserLocationUpdatedIntegrationEvent>
    {


        public async Task Handle(UserLocationUpdatedIntegrationEvent @event)
        {
            await Task.Delay(0);
        }

    }
}
