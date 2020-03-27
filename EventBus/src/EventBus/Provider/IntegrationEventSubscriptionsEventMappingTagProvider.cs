using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventBus.Provider
{
    public class IntegrationEventSubscriptionsEventMappingTagProvider : IIntegrationEventSubscriptionsEventMappingTagProvider
    {
        private readonly List<IntegrationEventSubscriptionEventMappingTag> _events;
        public IntegrationEventSubscriptionsEventMappingTagProvider(IOptions<IntegrationEventSubscriptionEventMappingTagOption> option)
        {
            _events = option.Value.EventsMappingTagBuilder.Build();
        }

        public Task<List<IntegrationEventSubscriptionEventMappingTag>> GetAllIntegrationEventSubscriptionEventMappingTagAsync()
        {
            return Task.FromResult(_events);
        }

        public Task<IntegrationEventSubscriptionEventMappingTag> GetIntegrationEventSubscriptionEventMappingTagAsync(Type eventType)
        {
            return Task.FromResult(_events.FirstOrDefault(p => p.EventType == eventType));
        }
    }
}
