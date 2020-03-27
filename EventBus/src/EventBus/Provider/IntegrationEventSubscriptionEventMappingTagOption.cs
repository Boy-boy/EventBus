using System;

namespace EventBus.Provider
{
    public class IntegrationEventSubscriptionEventMappingTagOption
    {
        private IntegrationEventSubscriptionEventMappingTagBuilder _eventsMappingTagBuilder = new IntegrationEventSubscriptionEventMappingTagBuilder();

        public IntegrationEventSubscriptionEventMappingTagBuilder EventsMappingTagBuilder => _eventsMappingTagBuilder;

        public void AddEventMappingTagBuilder(Action<IntegrationEventSubscriptionEventMappingTagBuilder> configureBuilder)
        {
            if (configureBuilder == null)
            {
                throw new ArgumentNullException(nameof(configureBuilder));
            }
            var build = new IntegrationEventSubscriptionEventMappingTagBuilder();
            configureBuilder(build);
            _eventsMappingTagBuilder = build;
        }
    }
}
