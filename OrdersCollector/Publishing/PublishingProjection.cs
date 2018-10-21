using System;
using System.Threading;
using System.Threading.Tasks;
using Marten;
using Marten.Events.Projections;
using Marten.Events.Projections.Async;
using Marten.Storage;
using Microsoft.AspNetCore.SignalR;
using OrdersCollector.Domain.Local.Events.V1;
using OrdersCollector.Domain.Order.Events.V1;
using Serilog;

namespace OrdersCollector.Publishing
{
    public class PublishingProjection : IProjection
    {
        private static readonly ILogger Logger = Log.ForContext<PublishingProjection>();
        
        private readonly IHubContext<EventsHub> _hubContext;
        public AsyncOptions AsyncOptions { get; } = new AsyncOptions();

        public Type[] Consumes { get; } =
        {
            typeof(NewOrderStarted),
            typeof(OrderItemAdded),
            typeof(OrderResponsiblePersonSelected),
            typeof(OrderResponsiblePersonRemoved),
            typeof(OrderItemRemoved),
            typeof(LocalAdded),
            typeof(LocalRemoved),
            typeof(LocalRenamed),
            typeof(LocalAliasAdded),
            typeof(LocalAliasRemoved)
        };

        public PublishingProjection(IHubContext<EventsHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task ApplyAsync(IDocumentSession session, EventPage page, CancellationToken token)
        {
            foreach (var @event in page.Events)
            {
                var type = @event.Data?.GetType().Name;
                if (type != null)
                {
                    Logger.Debug("Publishing event of type {EventType}", @event.Data.GetType());
                    await _hubContext.Clients.All.SendAsync("event", new { Type = type, @event.Data}, cancellationToken: token);
                }
            }
        }

        public void EnsureStorageExists(ITenant tenant) { }

        public void Apply(IDocumentSession session, EventPage page)
        {
            throw new NotImplementedException("Only async version is used");
        }
    }
}