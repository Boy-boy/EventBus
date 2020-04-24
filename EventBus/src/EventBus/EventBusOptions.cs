using System;
using System.Collections.Generic;

namespace EventBus
{
    public class EventBusOptions
    {
        private readonly IList<IntegrationEventSubscriptionBuildEventTagBuilder> _eventTagBuilders = new List<IntegrationEventSubscriptionBuildEventTagBuilder>();

        public IEnumerable<IntegrationEventSubscriptionBuildEventTagBuilder> EventTagBuilders => _eventTagBuilders;

        public void AddBuildEventTag(Action<IntegrationEventSubscriptionBuildEventTagBuilder> configureBuilder)
        {
            if (configureBuilder == null)
                throw new ArgumentNullException(nameof(configureBuilder));
            var authenticationSchemeBuilder = new IntegrationEventSubscriptionBuildEventTagBuilder();
            configureBuilder(authenticationSchemeBuilder);
            _eventTagBuilders.Add(authenticationSchemeBuilder);
        }
    }
}
