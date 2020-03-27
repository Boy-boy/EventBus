﻿using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using EventBus.Provider;

namespace EventBus
{
    public class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionsManager
    {
        private readonly IIntegrationEventSubscriptionsEventMappingTagProvider _provider;
        private readonly Dictionary<string, List<Type>> _handlers;
        private readonly Dictionary<string, Type> _eventTypes;
        public event EventHandler<string> OnEventRemoved;

        public InMemoryEventBusSubscriptionsManager(IIntegrationEventSubscriptionsEventMappingTagProvider provider)
        {
            _provider = provider;
            _handlers = new Dictionary<string, List<Type>>();
            _eventTypes = new Dictionary<string, Type>();
        }

        public bool IsEmpty => !_handlers.Keys.Any();
        public void Clear() => _handlers.Clear();

        #region AddSubscription
        public void AddSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = GetEventName<T>();
            var eventKey = GetEventKey<T>();
            DoAddSubscription(typeof(TH), eventKey, eventName);

            if (!_eventTypes.ContainsKey(eventKey))
            {
                _eventTypes.Add(eventKey, typeof(T));
            }
        }
        private void DoAddSubscription(Type handlerType, string eventKey, string eventName)
        {
            if (!HasSubscriptionsForEvent(eventKey))
            {
                _handlers.Add(eventKey, new List<Type>());
            }
            if (_handlers[eventKey].Any(s => s == handlerType))
            {
                throw new ArgumentException(
                    $"Handler Type {handlerType.Name} already registered for '{eventName}'", nameof(handlerType));
            }
            _handlers[eventKey].Add(handlerType);
        }
        #endregion

        #region RemoveSubscription
        public void RemoveSubscription<T, TH>()
            where TH : IIntegrationEventHandler<T>
            where T : IntegrationEvent
        {
            var eventKey = GetEventKey<T>();
            var handlerToRemove = DoFindSubscriptionToRemove(eventKey, typeof(TH));
            DoRemoveHandler(eventKey, handlerToRemove);
        }
        private Type DoFindSubscriptionToRemove(string eventKey, Type handlerType)
        {
            return !HasSubscriptionsForEvent(eventKey) ? null : _handlers[eventKey].SingleOrDefault(s => s == handlerType);
        }
        private void DoRemoveHandler(string eventKey, Type subsToRemove)
        {
            if (subsToRemove == null) return;
            _handlers[eventKey].Remove(subsToRemove);
            if (_handlers[eventKey].Any()) return;
            _handlers.Remove(eventKey);
            RaiseOnEventRemoved(eventKey);
        }
        private void RaiseOnEventRemoved(string eventKey)
        {
            var handler = OnEventRemoved;
            handler?.Invoke(this, eventKey);
            _eventTypes.TryGetValue(eventKey, out var eventType);
            if (eventType != null)
            {
                _eventTypes.Remove(eventKey);
            }
        }
        #endregion

        public IEnumerable<Type> GetHandlersForEvent<T>() where T : IntegrationEvent
        {
            var eventKey = GetEventKey<T>();
            return GetHandlersForEvent(eventKey);
        }

        public IEnumerable<Type> GetHandlersForEvent(string eventKey) => _handlers[eventKey];

        public bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent
        {
            var eventKey = GetEventKey<T>();
            return HasSubscriptionsForEvent(eventKey);
        }

        public bool HasSubscriptionsForEvent(string eventKey) => _handlers.ContainsKey(eventKey);

        public Type GetEventTypeByKey(string eventKey)
        {
            _eventTypes.TryGetValue(eventKey, out var eventType);
            return eventType;
        }

        public string GetEventName<T>()
        {
            return typeof(T).Name;
        }

        public string GetEventKey<T>()
        {
            var eventName = GetEventName<T>();
            var eventTag = _provider.GetIntegrationEventSubscriptionEventMappingTagAsync(typeof(T)).Result?.EventTag;
            var eventKey = string.IsNullOrWhiteSpace(eventTag) ? eventName : $"{eventTag}_{eventName}";
            return eventKey;
        }
    }
}
