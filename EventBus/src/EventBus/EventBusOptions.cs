using System;
using System.Collections.Generic;

namespace EventBus
{
    public class EventBusOptions
    {
        private readonly IList<IntegrationEventSubscriptionEventMappingTagBuilder> _eventMappingTagBuilders = new List<IntegrationEventSubscriptionEventMappingTagBuilder>();

        public IEnumerable<IntegrationEventSubscriptionEventMappingTagBuilder> EventMappingTagBuilders => _eventMappingTagBuilders;

        public void AddEventMappingTag(Action<IntegrationEventSubscriptionEventMappingTagBuilder> configureBuilder)
        {
            if (configureBuilder == null)
                throw new ArgumentNullException(nameof(configureBuilder));
            var authenticationSchemeBuilder = new IntegrationEventSubscriptionEventMappingTagBuilder();
            configureBuilder(authenticationSchemeBuilder);
            _eventMappingTagBuilders.Add(authenticationSchemeBuilder);
        }
    }
}
