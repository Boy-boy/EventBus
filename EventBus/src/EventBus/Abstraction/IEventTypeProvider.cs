using System;

namespace EventBus.Abstraction
{
    public interface IEventTypeProvider
    {
        Type TryGetEventType(string eventKey);

        bool HasEventType(string eventKey);
    }
}
