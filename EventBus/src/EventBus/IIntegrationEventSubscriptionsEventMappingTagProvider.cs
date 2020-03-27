using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventBus.Provider;

namespace EventBus
{
    /// <summary>
    /// 集成事件订阅提供者
    /// </summary>
    public interface IIntegrationEventSubscriptionsEventMappingTagProvider
    {
        Task<List<IntegrationEventSubscriptionEventMappingTag>> GetAllIntegrationEventSubscriptionEventMappingTagAsync();

        Task<IntegrationEventSubscriptionEventMappingTag> GetIntegrationEventSubscriptionEventMappingTagAsync(Type eventType);
    }
}
