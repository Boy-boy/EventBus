using EventBus;

namespace WebAppSubscription.IntegrationEvents.Events1
{
    public class UserLocationUpdatedIntegrationEvent : IntegrationEvent
    {
        public string UserId { get; set; }


        public UserLocationUpdatedIntegrationEvent(string userId)
        {
            UserId = userId;
        }
    }
}
