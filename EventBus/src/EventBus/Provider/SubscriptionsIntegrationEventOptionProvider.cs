using Microsoft.Extensions.Options;

namespace EventBus.Provider
{
    public class SubscriptionsIntegrationEventOptionProvider
    {
        private readonly IOptions<SubscriptionsIntegrationEventOption> _option;

        public SubscriptionsIntegrationEventOptionProvider(IOptions<SubscriptionsIntegrationEventOption> option)
        {
            _option = option;
        }

        public SubscriptionsIntegrationEventOption GetSubscriptionsIntegrationEventOption()
        {
            return _option.Value;
        }
    }
}
