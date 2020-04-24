using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventBus
{
    public interface IIntegrationEventSubscriptionBuildEventTagProvider
    {
       Task<IntegrationEventSubscriptionBuildEventTag> GetEventMappingTagAsync(Type evenType);
       Task<IEnumerable<IntegrationEventSubscriptionBuildEventTag>> GetAllEventMappingTagsAsync();
   }
}
