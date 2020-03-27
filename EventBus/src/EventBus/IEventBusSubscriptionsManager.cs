using System;
using System.Collections.Generic;

namespace EventBus
{
    /// <summary>
    /// 消息订阅管理器
    /// </summary>
    public interface IEventBusSubscriptionsManager
    {
        bool IsEmpty { get; }
        event EventHandler<string> OnEventRemoved;
        
        void AddSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        void RemoveSubscription<T, TH>()
            where TH : IIntegrationEventHandler<T>
            where T : IntegrationEvent;
        
        bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent;
        bool HasSubscriptionsForEvent(string eventName);
        Type GetEventTypeByName(string eventName);
        void Clear();
        IEnumerable<Type> GetHandlersForEvent<T>() where T : IntegrationEvent;
        IEnumerable<Type> GetHandlersForEvent(string eventName);
        string GetEventKey<T>();
        string GetEventName<T>();
        
    }
}
