using System;
using System.Collections.Generic;

namespace EventBus.Abstraction
{
    /// <summary>
    /// 消息订阅管理器
    /// </summary>
    public interface IEventBusSubscriptionsManager
    {
        ICollection<IIntegrationEventHandler> GetHandlers(string eventKey);

        Type TryGetEventTypeByEventKey(string eventKey);

        bool HasEventTypeByEventKey(string eventKey);
    }
}
