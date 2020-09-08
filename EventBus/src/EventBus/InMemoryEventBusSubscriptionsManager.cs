using EventBus.Abstraction;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace EventBus
{
    public class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionsManager
    {
        private readonly IEventHandlersProvider _eventHandlersProvider;

        public event EventHandler<string> OnEventRemoved;

        private ConcurrentDictionary<string, List<EventHandlerWrapper>> _handlers { get; }
        private ConcurrentDictionary<string, Type> _eventTypes { get; }

        public InMemoryEventBusSubscriptionsManager(
            IEventHandlersProvider eventHandlersProvider)
        {
            _eventHandlersProvider = eventHandlersProvider;
            _handlers = new ConcurrentDictionary<string, List<EventHandlerWrapper>>();
            _eventTypes = new ConcurrentDictionary<string, Type>();
        }
        public ICollection<EventHandlerWrapper> GetHandlers(string eventName)
        {
            _handlers.TryGetValue(eventName, out var handles);
            return handles;
        }

        #region AddSubscription
        public void AddSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            TryAddSubscriptionEventTypes<T>();
            TryAddSubscriptionHandlers<T, TH>();
        }

        private void TryAddSubscriptionHandlers<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = EventNameAttribute.GetNameOrDefault(typeof(T));
            var handlerType = typeof(TH);
            if (!IncludeSubscriptionsHandlesForEventName(eventName))
            {
                _handlers.GetOrAdd(eventName, new List<EventHandlerWrapper>());
            }
            if (_handlers[eventName].Any(s => s.EventHandlerType == handlerType))
            {
                throw new ArgumentException(
                    $"Handler Type {handlerType.Name} already registered for '{eventName}'", nameof(handlerType));
            }
            _handlers[eventName].Add(_eventHandlersProvider.GetHandler(handlerType));
        }

        private void TryAddSubscriptionEventTypes<T>()
            where T : IntegrationEvent
        {
            var eventType = typeof(T);
            var eventName = EventNameAttribute.GetNameOrDefault(eventType);
            if (_eventTypes.ContainsKey(eventName))
            {
                //duplicate event key
                if (_eventTypes[eventName] != eventType)
                {
                    throw new ArgumentException(
                        $"Event name {eventName} already exists,please make sure the event key is unique");
                }
            }
            else
            {
                _eventTypes[eventName] = eventType;
            }
        }
        #endregion

        #region RemoveSubscription
        public void RemoveSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = EventNameAttribute.GetNameOrDefault(typeof(T));
            var handlerToRemove = TryFindSubscriptionToRemove(eventName, typeof(TH));
            TryRemoveHandler(eventName, handlerToRemove);
        }

        private EventHandlerWrapper TryFindSubscriptionToRemove(string eventName, Type handlerType)
        {
            return !IncludeSubscriptionsHandlesForEventName(eventName) ? null : _handlers[eventName].SingleOrDefault(s => s.EventHandlerType == handlerType);
        }
        private void TryRemoveHandler(string eventName, EventHandlerWrapper subsToRemove)
        {
            if (subsToRemove == null) return;
            _handlers[eventName].Remove(subsToRemove);
            if (_handlers[eventName].Any()) return;
            _handlers.TryRemove(eventName, out _);
            RaiseOnEventRemoved(eventName);
        }
        private void RaiseOnEventRemoved(string eventName)
        {
            var handler = OnEventRemoved;
            handler?.Invoke(this, eventName);
            _eventTypes.TryGetValue(eventName, out var eventType);
            if (eventType != null)
            {
                _eventTypes.TryRemove(eventName, out _);
            }
        }
        #endregion

        public bool IncludeSubscriptionsHandlesForEventName(string eventName) => _handlers.ContainsKey(eventName);

        public bool IncludeEventTypeForEventName(string eventName) => _eventTypes.ContainsKey(eventName);

        public Type TryGetEventTypeForEventName(string eventName)
        {
            _eventTypes.TryGetValue(eventName, out var eventType);
            return eventType;
        }

        public void Dispose()
        {
            _handlers?.Clear();
            _eventTypes?.Clear();
        }
    }
}
