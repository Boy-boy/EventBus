using System;
using System.Threading.Tasks;

namespace EventBus.Abstraction
{
    public interface IEventBus: IDisposable
    {
        Task Publish(IntegrationEvent @event);

        void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        void UnSubscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;
    }
}
