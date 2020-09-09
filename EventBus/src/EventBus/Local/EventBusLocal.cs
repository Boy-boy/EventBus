using EventBus.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EventBus.Local
{
    public class EventBusLocal : IEventBus
    {
        private readonly IEventBusSubscriptionsManager _subsManager;
        private readonly IEventHandlerFactory _eventHandlerFactory;

        public EventBusLocal(IEventBusSubscriptionsManager subsManager,
            IEventHandlerFactory eventHandlerFactory)
        {
            _subsManager = subsManager;
            _eventHandlerFactory = eventHandlerFactory;
        }

        public async Task Publish(IntegrationEvent @event)
        {
            var exceptions = new List<Exception>();
            var eventType = @event.GetType();
            var eventName = EventNameAttribute.GetNameOrDefault(eventType);
            if (_subsManager.IncludeEventTypeForEventName(eventName))
            {
                var eventHandleTypes = _subsManager.GetHandlerTypes(eventName);
                foreach (var eventHandleType in eventHandleTypes)
                {
                    try
                    {
                        var handlerInstance = _eventHandlerFactory.GetHandler(eventHandleType);
                        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                        await Task.Yield();
                        await (Task)concreteType.GetMethod("Handle").Invoke(handlerInstance, new object[] { @event });
                    }
                    catch (TargetInvocationException ex)
                    {
                        exceptions.Add(ex.InnerException);
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }
            }
            else
            {
                throw new AggregateException($"No subscription for local memory event: {eventName}");
            }
            if (exceptions.Any())
            {
                throw new AggregateException(
                    "More than one error has occurred while triggering the event: " + eventType, exceptions);
            }
        }

        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            _subsManager.AddSubscription<T, TH>();
        }

        public void UnSubscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            _subsManager.RemoveSubscription<T, TH>();
        }

        public void Dispose()
        {
            _subsManager?.Dispose();
        }
    }
}
