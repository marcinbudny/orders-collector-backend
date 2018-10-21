using System.Threading.Tasks;
using OrdersCollector.Domain.Local;
using OrdersCollector.Domain.Local.Commands.V1;

namespace OrdersCollector.Tests.Integration
{
    public class LocalFixture : Fixture
    {
        public Task Local_has_been_added(string id, string name) =>
            PostToSutApi("locals/add-local", new AddLocal {Id = id, Name = name});
        
        public Task Local_has_been_removed(string id) => 
            PostToSutApi("locals/remove-local", new RemoveLocal {Id = id});
        
        public Task Local_alias_has_been_added(string localId, string alias) => 
            PostToSutApi("locals/add-alias", new AddLocalAlias {LocalId = localId, Alias = alias});

        public Task Local_alias_has_been_removed(string localId, string alias) => 
            PostToSutApi("locals/remove-local-alias", new RemoveLocalAlias {LocalId = localId, Alias = alias});

        public Task Local_has_been_renamed(string id, string newName) => 
            PostToSutApi("locals/rename-local", new RenameLocal {Id = id, NewName = newName});
        
    }
}