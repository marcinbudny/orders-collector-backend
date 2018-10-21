using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using OrdersCollector.Domain.Order;
using OrdersCollector.Domain.Order.Events.V1;
using OrdersCollector.Tests.Utils;
using Xunit;

namespace OrdersCollector.Tests.Integration
{
    public class OrderTests : OrderFixture
    {
        private const string LocalId = "Masala House";
        private const string Person1 = "Marcin B";
        private static readonly DateTime Item1Date = "2018-01-02".ToUtcDateTime();
        private const string Item1 = "Tikka masala kurczak";
        private const string Person2 = "Łukasz G";
        private const string Item2 = "Vindaloo kurczak";
        private static readonly DateTime Item2Date = "2018-01-02 01:00:00".ToUtcDateTime();       

        private async Task<(OrderId orderId, OrderDate orderDate)> Items_have_been_ordered()
        {
            It_is_now(Item1Date);

            await Item_has_been_ordered(LocalId, Person1, Item1);

            It_is_now(Item2Date);

            return await Item_has_been_ordered(LocalId, Person2, Item2);
        }

        [Fact]
        public async Task Can_order_items()
        {
            var (orderId, orderDate) = await Items_have_been_ordered();

            await Event_stream_should_have_these_events(orderId.Value,
                new NewOrderStarted(orderId, LocalId, orderDate),
                new OrderItemAdded(orderId, Item1, Person1, Item1Date),
                new OrderItemAdded(orderId, Item2, Person2, Item2Date));


            await Wait_for_projection_to_process_events();

            await Read_model_should_have_this_order_document(orderId, new Reads.Document.Order
            {
                Id = orderId.Value,
                LocalId = LocalId,
                Date = orderDate,
                Items = new List<Reads.Document.OrderItem>
                {
                    new Reads.Document.OrderItem
                    {
                        PersonName = Person1,
                        ItemName = Item1,
                        AddedAt = Item1Date
                    },
                    new Reads.Document.OrderItem
                    {
                        PersonName = Person2,
                        ItemName = Item2,
                        AddedAt = Item2Date
                    }
                }
            });

            await Read_model_should_have_this_in_order_table(
                new Dictionary<string, object>
                {
                    ["id"] = orderId.Value,
                    ["date"] = orderDate.Value,
                    ["local_id"] = LocalId,
                    ["responsible_person"] = null,
                });

            await Read_model_should_have_this_in_order_item_table(
                new Dictionary<string, object>
                {
                    ["person_name"] = Person1,
                    ["item_name"] = Item1,
                    ["added_at"] = Item1Date,
                    ["order_id"] = orderId.Value
                }, new Dictionary<string, object>
                {
                    ["person_name"] = Person2,
                    ["item_name"] = Item2,
                    ["added_at"] = Item2Date,
                    ["order_id"] = orderId.Value
                });
        }
        
        [Fact]
        public async Task Can_select_responsible_person()
        {
            It_is_now(Item1Date);

            var (orderId, orderDate) = await Item_has_been_ordered(LocalId, Person1, Item1);

            await Responsible_person_has_been_selected(orderId);

            await Wait_for_projection_to_process_events();

            await Read_model_should_have_this_order_document(orderId, o => o?.ResponsiblePerson.Should().Be(Person1));

            await Read_model_should_have_this_in_order_table(
                new Dictionary<string, object>
                {
                    ["id"] = orderId.Value,
                    ["date"] = orderDate.Value,
                    ["local_id"] = LocalId,
                    ["responsible_person"] = Person1,
                });
        }        
        
        [Fact]
        public async Task Can_remove_item_and_responsible_person()
        {
            It_is_now(Item1Date);

            var (orderId, orderDate) = await Item_has_been_ordered(LocalId, Person1, Item1);

            await Responsible_person_has_been_selected(orderId);

            await Item_has_been_removed(orderId, Person1);

            await Wait_for_projection_to_process_events();

            await Read_model_should_have_this_order_document(orderId, o =>
            {
                o.ResponsiblePerson.Should().BeNull();
                o.Items.Should().BeEmpty();
            });

            await Read_model_should_have_this_in_order_table(
                new Dictionary<string, object>
                {
                    ["id"] = orderId.Value,
                    ["date"] = orderDate.Value,
                    ["local_id"] = LocalId,
                    ["responsible_person"] = null,
                });

            await Read_model_order_item_table_should_be_empty();
        }
    }
}