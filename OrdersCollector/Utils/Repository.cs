using System.Linq;
using System.Threading.Tasks;
using Marten;

namespace OrdersCollector.Utils
{
    public interface IRepository<TAggregateRoot> 
        where TAggregateRoot : AggregateRoot<string>, new()
    {
        Task<TAggregateRoot> LoadById(string id);  

        Task Save(TAggregateRoot aggregateRoot); 
    }
    
    public class Repository<TAggregateRoot> : IRepository<TAggregateRoot>
        where TAggregateRoot : AggregateRoot<string>, new()
    {
        private readonly IDocumentStore _store;

        public Repository(IDocumentStore store) => _store = store;

        public async Task<TAggregateRoot> LoadById(string id)
        {
            using(var session = _store.OpenSession())
            {
                // TODO: paging
                var events = await session.Events.FetchStreamAsync(id);
                if(events.Any())
                {
                    var aggregateRoot = new TAggregateRoot();
                    var lastVersion = events.Last().Version;

                    aggregateRoot.ReplayAll(events.Select(e => e.Data), lastVersion);
                    return aggregateRoot;
                }

                return null;
            }
        }

        public async Task Save(TAggregateRoot aggregateRoot)
        {
            using(var session = _store.OpenSession())
            {
                session.Events.Append(
                    aggregateRoot.Id, 
                    aggregateRoot.Version, 
                    aggregateRoot.PublishedEvents.ToArray());
                    
                await session.SaveChangesAsync();
            }
        }
    }
}