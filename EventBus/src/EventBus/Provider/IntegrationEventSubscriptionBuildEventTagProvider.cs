using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace EventBus
{
    public class IntegrationEventSubscriptionBuildEventTagProvider : IIntegrationEventSubscriptionBuildEventTagProvider
    {

        private readonly IList<IntegrationEventSubscriptionBuildEventTag> _eventTags = new List<IntegrationEventSubscriptionBuildEventTag>();
        public IEnumerable<IntegrationEventSubscriptionBuildEventTag> EventTags => _eventTags;
        private readonly EventBusOptions _options;

        public IntegrationEventSubscriptionBuildEventTagProvider(IOptions<EventBusOptions> options)
        {
            _options = options.Value;
            foreach (var builder in this._options.EventTagBuilders)
                _eventTags.Add(builder.Build());
        }
        public  Task<IntegrationEventSubscriptionBuildEventTag> GetEventMappingTagAsync(Type evenType)
        {
            return Task.FromResult(EventTags.LastOrDefault(p => p.EventType == evenType));
        }

        public  Task<IEnumerable<IntegrationEventSubscriptionBuildEventTag>> GetAllEventMappingTagsAsync()
        {
            return Task.FromResult(EventTags);
        }
    }
}
