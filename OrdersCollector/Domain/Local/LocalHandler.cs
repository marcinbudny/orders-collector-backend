using System.Threading.Tasks;
using Marten;
using Marten.Services;
using OrdersCollector.Domain.Local.Commands.V1;
using OrdersCollector.Domain.Local.Events.V1;
using OrdersCollector.Utils;

namespace OrdersCollector.Domain.Local
{
    public class LocalHandler
    {
        // provide abstraction
        private readonly IDocumentStore _store;

        public LocalHandler(IDocumentStore store)
        {
            _store = store;
        }

        private async Task AppendEvent(string streamId, object @event, int expectedVersion = 0)
        {
            using (var session = _store.OpenSession())
            {
                session.Events.Append(streamId, expectedVersion, @event);

                await session.SaveChangesAsync();
            }
        } 

        public async Task Handle(AddLocal command)
        {
            IdValue.ValidateOrThrow(command.Id);
            LocalName.ValidateOrThrow(command.Name);
            try
            {
                await AppendEvent(command.Id, new LocalAdded(command.Id, command.Name), expectedVersion: 1);
            }
            catch(EventStreamUnexpectedMaxEventIdException)
            {
                throw new DomainException($"Local with id {command.Id} already exists", ErrorCode.LocalAlreadyExists);    
            }
        }
        
        public async Task Handle(RenameLocal command)
        {
            IdValue.ValidateOrThrow(command.Id);
            LocalName.ValidateOrThrow(command.NewName);
            await AppendEvent(command.Id, new LocalRenamed(command.Id, command.NewName));
        }
        
        public async Task Handle(RemoveLocal command)
        {
            IdValue.ValidateOrThrow(command.Id);
            await AppendEvent(command.Id, new LocalRemoved(command.Id));
        }

        public async Task Handle(AddLocalAlias command)
        {
            IdValue.ValidateOrThrow(command.LocalId);
            LocalName.ValidateOrThrow(command.Alias);
            await AppendEvent(command.LocalId, new LocalAliasAdded(command.LocalId, command.Alias));
        }

        public async Task Handle(RemoveLocalAlias command)
        {
            IdValue.ValidateOrThrow(command.LocalId);
            LocalName.ValidateOrThrow(command.Alias);
            await AppendEvent(command.LocalId, new LocalAliasRemoved(command.LocalId, command.Alias));
        }
    }
}