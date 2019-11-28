using EventBus;

namespace WebAppSubscription.IntegrationEvents.Events
{
    public class UserLocationUpdatedIntegrationEvent : IntegrationEvent
    {
        public int Age { get; set; }
    }
}
