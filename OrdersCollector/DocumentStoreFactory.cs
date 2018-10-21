using Marten;
using Marten.Events;
using Microsoft.AspNetCore.SignalR;
using OrdersCollector.Domain.Local.Events.V1;
using OrdersCollector.Domain.Order.Events.V1;
using OrdersCollector.Publishing;
using OrdersCollector.Reads.Document;
using OrdersCollector.Reads.Relational;

namespace OrdersCollector
{
    public static class DocumentStoreFactory
    {
        public static DocumentStore Create(string connectionString, IHubContext<EventsHub> hubContext) 
        {
            return DocumentStore.For(cfg =>
            {
                cfg.Connection(connectionString);
                cfg.Events.StreamIdentity = StreamIdentity.AsString;

                // TODO: autodiscover event types
                cfg.Events.AddEventType(typeof(NewOrderStarted));
                cfg.Events.AddEventType(typeof(OrderItemAdded));
                cfg.Events.AddEventType(typeof(OrderResponsiblePersonSelected));
                cfg.Events.AddEventType(typeof(OrderResponsiblePersonRemoved));
                cfg.Events.AddEventType(typeof(OrderItemRemoved));
                cfg.Events.AddEventType(typeof(LocalAdded));
                cfg.Events.AddEventType(typeof(LocalRemoved));
                cfg.Events.AddEventType(typeof(LocalRenamed));
                cfg.Events.AddEventType(typeof(LocalAliasRemoved));
                cfg.Events.AddEventType(typeof(LocalAliasAdded));

                cfg.Events.AsyncProjections.AggregateStreamsWith<Reads.Document.Order>();
                cfg.Events.AsyncProjections.Add(new RelationalProjection(connectionString, new SchemaCreator(connectionString)));
                cfg.Events.AsyncProjections.Add(new LocalProjection());
                cfg.Events.AsyncProjections.Add(new PublishingProjection(hubContext));
            });
        }
    }
}