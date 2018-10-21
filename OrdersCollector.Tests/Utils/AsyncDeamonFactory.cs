using Marten;
using Marten.Events.Projections.Async;

namespace OrdersCollector.Tests.Utils
{
    public static class AsyncDeamonFactory
    {
        public static IDaemon CreateAndStart(IDocumentStore store) 
        {
            var daemon = store.BuildProjectionDaemon();
            daemon.StartAll();
            return daemon;
        } 
    }
}