using System;
using FluentAssertions;
using OrdersCollector.Domain;
using OrdersCollector.Domain.Order;
using OrdersCollector.Domain.Order.Events.V1;
using OrdersCollector.Tests.Utils;
using Xunit;
using static OrdersCollector.Tests.Unit.TestAggregateEntryPoint;
using static OrdersCollector.Tests.Utils.DontCare;


namespace OrdersCollector.Tests.Unit
{
    public class OrderTests
    {
        static readonly string localId = "Masala House";
        static readonly OrderDate orderDate = OrderDate.From("2018-02-01".ToDateTime());
        static readonly OrderId orderId = OrderId.From(localId, orderDate);
        
        [Fact]
        public void Can_order_item()
        {
            var now = DateTime.Now;
            
            Test<Order>()
                
                .WhenCreated(() => new Order(orderId, localId, orderDate))
                
                .When(o => o.OrderNewItem("Tikka masala", "Marcin B", addedAt: now))
                
                .Expect(
                    new NewOrderStarted(orderId, localId, orderDate),
                    new OrderItemAdded(orderId, "Tikka masala", "Marcin B", addedAt: now)
                    );
        }

        [Fact]
        public void Should_not_allow_to_order_twice_for_same_person()
        {
            Test<Order>()
                
                .Given(
                    new NewOrderStarted(orderId, localId, orderDate),
                    new OrderItemAdded(orderId, "Tikka masala", "Marcin B", addedAt: SomeDate))
                
                .When(o => o.OrderNewItem("Rogan josh", "Marcin B", addedAt: SomeDate))
                
                .ExpectThrows<DomainException>(e => e.ErrorCode == ErrorCode.PersonAlreadyOrdered);
        }

        [Fact]
        public void Can_select_responsible_person_if_one_item_only()
        {
            Test<Order>()
                
                .Given(
                    new NewOrderStarted(orderId, localId, orderDate),
                    new OrderItemAdded(orderId, "Tikka masala", "Marcin B", addedAt: SomeDate))
                
                .When(o => o.SelectResponsiblePerson())
                
                .Expect(new OrderResponsiblePersonSelected(orderId, "Marcin B"));
        }
        
        [Fact]
        public void Can_select_responsible_person()
        {
            Test<Order>()
                
                .Given(
                    new NewOrderStarted(orderId, localId, orderDate),
                    new OrderItemAdded(orderId, "Tikka masala", "Marcin B", addedAt: SomeDate),
                    new OrderItemAdded(orderId, "Rogan josh", "Lukasz G", addedAt: SomeDate))
                
                .When(o => o.SelectResponsiblePerson())
                
                .Expect<OrderResponsiblePersonSelected>(e => e.PersonName.Should().BeOneOf("Marcin B", "Lukasz G"));
        }

        [Fact]
        public void Should_throw_if_responsible_person_already_selected()
        {            
            Test<Order>()
                
                .Given(
                    new NewOrderStarted(orderId, localId, orderDate),
                    new OrderItemAdded(orderId, "Tikka masala", "Marcin B", addedAt: SomeDate),
                    new OrderResponsiblePersonSelected(orderId, "Marcin B"))
                
                .When(o => o.SelectResponsiblePerson())
                
                .ExpectThrows<DomainException>(e => e.ErrorCode == ErrorCode.ResponsiblePersonAlreadySelected);
            
        }

        [Fact]
        public void Can_remove_item()
        {
            Test<Order>()
                .Given(
                    new NewOrderStarted(orderId, localId, orderDate),
                    new OrderItemAdded(orderId, "Tikka masala", "Marcin B", addedAt: SomeDate),
                    new OrderItemAdded(orderId, "Rogan josh", "Lukasz G", addedAt: SomeDate))
                
                .When(o => o.RemoveItem(from: "Marcin B"))
                
                .Expect(new OrderItemRemoved(orderId, "Tikka masala", "Marcin B"));
        }
        
        [Fact]
        public void Should_throw_if_removing_item_from_person_who_didnt_order()
        {
            Test<Order>()
                .Given(
                    new NewOrderStarted(orderId, localId, orderDate),
                    new OrderItemAdded(orderId, "Tikka masala", "Marcin B", addedAt: SomeDate),
                    new OrderItemAdded(orderId, "Rogan josh", "Lukasz G", addedAt: SomeDate))
                
                .When(o => o.RemoveItem(from: "Rafal Z"))
                
                .ExpectThrows<DomainException>(e => e.ErrorCode == ErrorCode.CannotRemoveItemPersonDidntOrder);
        }
        
        [Fact]
        public void Should_select_new_responsible_person_when_removing_current_persons_item()
        {
            Test<Order>()
                .Given(
                    new NewOrderStarted(orderId, localId, orderDate),
                    new OrderItemAdded(orderId, "Tikka masala", "Marcin B", addedAt: SomeDate),
                    new OrderItemAdded(orderId, "Rogan josh", "Lukasz G", addedAt: SomeDate),
                    new OrderResponsiblePersonSelected(orderId, "Marcin B"))
                
                .When(o => o.RemoveItem(from: "Marcin B"))
                
                .Expect(
                    new OrderItemRemoved(orderId, "Tikka masala", "Marcin B"),
                    new OrderResponsiblePersonSelected(orderId, "Lukasz G"));
        }

        [Fact]
        public void Should_remove_responsible_person_if_last_item_removed()
        {
            Test<Order>()
                .Given(
                    new NewOrderStarted(orderId, localId, orderDate),
                    new OrderItemAdded(orderId, "Tikka masala", "Marcin B", addedAt: SomeDate),
                    new OrderResponsiblePersonSelected(orderId, "Marcin B"))
                
                .When(o => o.RemoveItem(from: "Marcin B"))
                
                .Expect(
                    new OrderItemRemoved(orderId, "Tikka masala", "Marcin B"),
                    new OrderResponsiblePersonRemoved(orderId));
        }

    }
}