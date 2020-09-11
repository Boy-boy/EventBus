using System;
using System.Collections.Generic;

namespace EventBus.Abstraction
{
    /// <summary>
    /// 消息订阅管理器
    /// </summary>
    public interface IEventBusSubscriptionsManager : IDisposable
    {
        ICollection<Type> GetHandlerTypes(string eventName);

        event EventHandler<Type> OnEventRemoved;

        void AddSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>, new();

        void RemoveSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>, new();


        void AddSubscription(Type eventType, Type handlerType);

        void RemoveSubscription(Type eventType, Type handlerType);


        bool IncludeSubscriptionsHandlesForEventName(string eventName);

        bool IncludeEventTypeForEventName(string eventName);

        Type TryGetEventTypeForEventName(string eventName);

    }
}
