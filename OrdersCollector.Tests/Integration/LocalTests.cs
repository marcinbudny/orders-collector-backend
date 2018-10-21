using System;
using System.Threading.Tasks;
using FluentAssertions;
using Marten.Services;
using OrdersCollector.Domain;
using OrdersCollector.Domain.Local;
using OrdersCollector.Domain.Local.Events.V1;
using OrdersCollector.Reads.Document;
using Xunit;

namespace OrdersCollector.Tests.Integration
{
    public class LocalTests : LocalFixture
    {
        private const string LocalId = "masala";
        private const string LocalName = "Masala House";
        private const string Alias1 = "Masala";
        private const string Alias2 = "Masalahouse";


        private async Task Local_with_aliases_has_been_added()
        {
            await Local_has_been_added(LocalId, LocalName);
            await Local_alias_has_been_added(LocalId, Alias1);
            await Local_alias_has_been_added(LocalId, Alias2);
        }           
        
        [Fact]
        public async Task Can_add_local_with_alias()
        {
            await Local_with_aliases_has_been_added();
            
            await Event_stream_should_have_these_events(LocalId, 
                new LocalAdded(LocalId, LocalName),
                new LocalAliasAdded(LocalId, Alias1),
                new LocalAliasAdded(LocalId, Alias2)
                );

            await Task.Delay(1000);
            await Wait_for_projection_to_process_events();

            await Read_model_should_have_this_document<Local>(LocalId,
                l => l.Should().BeEquivalentTo(
                    new Local
                    {
                        Id = LocalId, 
                        Name = LocalName,
                        Aliases = { Alias1, Alias2 }
                    }));
        }

        [Fact]
        public async Task Can_rename_local()
        {
            await Local_has_been_added(LocalId, LocalName);

            await Local_has_been_renamed(LocalId, "Something else");
            
            await Event_stream_should_have_these_events(LocalId,
                new LocalAdded(LocalId, LocalName),
                new LocalRenamed(LocalId, "Something else")
                );

            await Wait_for_projection_to_process_events();

            await Read_model_should_have_this_document<Local>(LocalId,
                l => l.Should().BeEquivalentTo(
                    new Local
                    {
                        Id = LocalId, 
                        Name = "Something else",
                    }));
        }

                
        [Fact]
        public async Task Can_remove_alias()
        {
            await Local_with_aliases_has_been_added();

            await Local_alias_has_been_removed(LocalId, Alias1);
            
            await Event_stream_should_have_these_events(LocalId, 
                new LocalAdded(LocalId, LocalName),
                new LocalAliasAdded(LocalId, Alias1),
                new LocalAliasAdded(LocalId, Alias2),
                new LocalAliasRemoved(LocalId, Alias1)
            );

            await Wait_for_projection_to_process_events();

            await Read_model_should_have_this_document<Local>(LocalId,
                l => l.Should().BeEquivalentTo(
                    new Local
                    {
                        Id = LocalId, 
                        Name = LocalName,
                        Aliases = { Alias2 }
                    }));
        }

        [Fact]
        public async Task Can_remove_local()
        {
            await Local_with_aliases_has_been_added();

            await Local_has_been_removed(LocalId);
            
            await Event_stream_should_have_these_events(LocalId, 
                new LocalAdded(LocalId, LocalName),
                new LocalAliasAdded(LocalId, Alias1),
                new LocalAliasAdded(LocalId, Alias2),
                new LocalRemoved(LocalId)
            );

            await Wait_for_projection_to_process_events();

            await Read_model_should_not_have_this_document<Local>(LocalId);
        }

        [Fact]
        public async Task Should_not_create_same_local_twice()
        {
            await Local_has_been_added(LocalId, LocalName);

            Func<Task> sutAction = async () => await Local_has_been_added(LocalId, LocalName);

            sutAction.Should().Throw<TestApiCalErrorException>().Where(
                e => e.Error.ErrorCode == ErrorCode.LocalAlreadyExists);
        }
    }
}