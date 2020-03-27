using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace EventBus
{
    public class IntegrationEventSubscriptionEventMappingTagProvider: IIntegrationEventSubscriptionEventMappingTagProvider
    {

        private readonly IList<IntegrationEventSubscriptionEventMappingTag> _eventMappingTags = new List<IntegrationEventSubscriptionEventMappingTag>();
        public IEnumerable<IntegrationEventSubscriptionEventMappingTag> EventMappingTags => _eventMappingTags;
        private readonly EventBusOptions _options;

        public IntegrationEventSubscriptionEventMappingTagProvider(IOptions<EventBusOptions> options)
        {
            _options = options.Value;
            foreach (var builder in this._options.EventMappingTagBuilders)
                _eventMappingTags.Add(builder.Build());
        }
        public  Task<IntegrationEventSubscriptionEventMappingTag> GetEventMappingTagAsync(Type evenType)
        {
            return Task.FromResult(EventMappingTags.FirstOrDefault(p => p.EventType == evenType));
        }

        public  Task<IEnumerable<IntegrationEventSubscriptionEventMappingTag>> GetAllEventMappingTagsAsync()
        {
            return Task.FromResult(EventMappingTags);
        }
    }
}
