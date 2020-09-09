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

        event EventHandler<string> OnEventRemoved;

        void AddSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>, new();

        void RemoveSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>, new();


        bool IncludeSubscriptionsHandlesForEventName(string eventName);

        bool IncludeEventTypeForEventName(string eventName);

        Type TryGetEventTypeForEventName(string eventName);

    }
}
