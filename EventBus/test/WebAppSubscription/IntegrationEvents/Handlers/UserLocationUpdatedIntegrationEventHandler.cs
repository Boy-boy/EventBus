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
            await Task.Yield();
        }

    }
    public class UserLocationUpdatedIntegrationEventHandler1
        : IIntegrationEventHandler<WebAppSubscription.IntegrationEvents.Events1.UserLocationUpdatedIntegrationEvent>
    {


        public async Task Handle(WebAppSubscription.IntegrationEvents.Events1.UserLocationUpdatedIntegrationEvent @event)
        {
            await Task.Yield();
        }

    }
}
