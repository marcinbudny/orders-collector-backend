namespace OrdersCollector.Domain
{
    // the least ugly workaround for Marten's lack of event metadata
    // using some sort of EventEnvelope<TEvent> requires whole bunch of reflection
    public class EventMetadata
    {
        public string RaisedByCommandId { get; set; }
    }
}