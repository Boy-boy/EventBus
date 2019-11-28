using System;

namespace EventBus
{
    public class IntegrationEvent
    {
        public IntegrationEvent()
        {
            Id = Guid.NewGuid();
            UtcCreation = DateTime.UtcNow;
            CreationTime = DateTime.Now;
        }

        public IntegrationEvent(string eventTag)
        :this()
        {
            EventTag = eventTag;
        }
        public IntegrationEvent(Guid id, DateTime createDate,string eventTag)
        {
            Id = id;
            CreationTime = createDate;
            EventTag = eventTag;
            UtcCreation = DateTime.UtcNow;
        }
        public Guid Id { get; }

        public DateTime UtcCreation { get; }

        public DateTime CreationTime { get; }

        public string EventTag { get; set; }
    }
}
