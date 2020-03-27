using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventBus
{
    public interface IIntegrationEventSubscriptionEventMappingTagProvider
   {
       Task<IntegrationEventSubscriptionEventMappingTag> GetEventMappingTagAsync(Type evenType);
       Task<IEnumerable<IntegrationEventSubscriptionEventMappingTag>> GetAllEventMappingTagsAsync();
   }
}
