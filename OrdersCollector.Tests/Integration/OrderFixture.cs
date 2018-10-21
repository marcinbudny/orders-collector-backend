using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using OrdersCollector.Domain.Order;
using OrdersCollector.Domain.Order.Commands.V1;
using OrdersCollector.Tests.Utils;
using OrdersCollector.Utils;

namespace OrdersCollector.Tests.Integration
{
    public class OrderFixture : Fixture
    {
        public static readonly DateTime DefaultDate = "2018-02-12".ToUtcDateTime();

        public OrderFixture() => It_is_now(DefaultDate);

        protected async Task<(OrderId orderId, OrderDate orderDate)> Item_has_been_ordered(
            string localId, string personName, string itemName)
        {
            var (orderId, orderDate) = GetOrderIdAndDate(TimeProvider.UtcNow, localId);
            
            await PostToSutApi("orders/order-item", new OrderItem
            {
                LocalId = localId,
                PersonName = personName,
                ItemName = itemName

            });

            return (orderId, orderDate);
        }

        protected async Task Item_has_been_removed(OrderId orderId, string personName)
        {
            await PostToSutApi("orders/remove-item", new RemoveItem
            {
                OrderId = orderId,
                PersonName = personName,
            });
        }

        protected async Task Responsible_person_has_been_selected(OrderId orderId)
        {
            await PostToSutApi("orders/select-responsible-person", new SelectResponsiblePerson {OrderId = orderId});
        }
        
        protected async Task Read_model_should_have_this_order_document(OrderId orderId, Action<Reads.Document.Order> verification)
        {
            using(var session = Store.OpenSession()) 
            {
                var order = await session.LoadAsync<Reads.Document.Order>(orderId.Value);
                verification(order);
            }
        }
        
        protected Task Read_model_should_have_this_order_document(OrderId orderId, Reads.Document.Order expected)
        {
            return Read_model_should_have_this_order_document(orderId, o => o.Should().BeEquivalentTo(expected));
        }

        protected async Task Read_model_should_have_this_in_order_table(params IDictionary<string, object>[] expected)
        {
            using (var conn = GetOpenConnection())
            {
                var orders = await conn.QueryAsync("select id, date, local_id, responsible_person from reads.order");
                orders.DapperReadModelShouldBeEquivalentTo(expected);
            }
        }
        
        protected async Task Read_model_should_have_this_in_order_item_table(params IDictionary<string, object>[] expected)
        {
            var items = await QueryOrderItemsTable();
            items.DapperReadModelShouldBeEquivalentTo(expected);
        }
        
        protected async Task Read_model_order_item_table_should_be_empty(params IDictionary<string, object>[] expected)
        {
            var items = await QueryOrderItemsTable();
            items.Should().BeEmpty();
        }

        private async Task<List<dynamic>> QueryOrderItemsTable()
        {
            using (var conn = GetOpenConnection())
            {
                return (await conn.QueryAsync("select person_name, item_name, added_at, order_id from reads.order_item")).ToList();
            }
        }
        
        private (OrderId, OrderDate) GetOrderIdAndDate(DateTime orderedDate, string localId)
        {
            var orderDate = OrderDate.From(orderedDate);
            var orderId = OrderId.From(localId, orderDate);
            return (orderId, orderDate);
        }
    }
}