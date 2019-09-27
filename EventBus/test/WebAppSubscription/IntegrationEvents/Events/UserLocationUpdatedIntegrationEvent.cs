using EventBus;

namespace WebAppSubscription.IntegrationEvents.Events
{
    public class UserLocationUpdatedIntegrationEvent : IntegrationEvent
    {
        public string Name { get; set; }


    }
}
