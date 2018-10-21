using System.Collections.Generic;

namespace OrdersCollector.Utils
{
    public abstract class AggregateRoot<TId> 
    {
        public TId Id { get; protected set; }

        public int Version { get; private set; } = 0;

        public List<object> PublishedEvents = new List<object>();

        public void Publish(object @event)
        {
            PublishedEvents.Add(@event);
            Version++;
            ApplyEvent(@event);
        }

        protected abstract void ApplyEvent(object @event);

        public void ReplayAll(IEnumerable<object> events, int versionAfterReplay)
        {
            events.ForEach(ApplyEvent);
            Version = versionAfterReplay;
        }
    }
}