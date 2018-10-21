using System;
using System.Collections.Generic;
using System.Linq;
using OrdersCollector.Domain.Local;
using OrdersCollector.Domain.Order.Events.V1;
using OrdersCollector.Utils;

namespace OrdersCollector.Domain.Order
{
    public class Order : AggregateRoot<string>
    {
        private OrderDate _date;
        private string _localId;
        private List<OrderItem> _items = new List<OrderItem>();
        private PersonName _responsiblePerson = null;

        public Order() { }

        public Order(OrderId orderId, string localId, OrderDate date)
        {
            Publish(new NewOrderStarted(orderId, localId, date));
        }

        public void OrderNewItem(ItemName itemName, PersonName personName, DateTime addedAt)
        {
            if(PersonAlreadyOrdered(personName))
                throw new DomainException($"Person {personName} has already ordered from {_localId}", ErrorCode.PersonAlreadyOrdered);
            
            Publish(new OrderItemAdded(Id, itemName, personName, addedAt));
        }

        private bool PersonAlreadyOrdered(PersonName personName) => _items.Any(i => i.PersonName == personName);

        public void SelectResponsiblePerson()
        {
            if(_items.Count == 0)
                throw new DomainException($"There is noone to select from in order {Id}", ErrorCode.NooneToSelectFrom);
            
            if(_responsiblePerson != null)
                throw new DomainException($"Responsible person already selected for order {Id}", ErrorCode.ResponsiblePersonAlreadySelected);
            
            var person = SelectRandomPerson();
            
            Publish(new OrderResponsiblePersonSelected(Id, person));
        }

        public void RemoveItem(PersonName @from)
        {
            var item = FindItemByPerson(@from);
            if(item == null) 
                throw new DomainException($"Cannot remove item from order {Id} from {@from}, because this person hasn't ordered anything", ErrorCode.CannotRemoveItemPersonDidntOrder);
            
           
            if (_responsiblePerson == @from)
            {
                if (OrderHasSingleItemAndItIsFrom(@from))
                {
                    Publish(new OrderResponsiblePersonRemoved(Id));
                }
                else
                {
                    var newResponsiblePerson = SelectRandomPerson(excluding: @from);
                    Publish(new OrderResponsiblePersonSelected(Id, newResponsiblePerson));
                }
            }
            
            Publish(new OrderItemRemoved(Id, item.ItemName, item.PersonName));
        }

        private OrderItem FindItemByPerson(PersonName person) => _items.FirstOrDefault(i => i.PersonName == person);
        
        private bool OrderHasSingleItemAndItIsFrom(PersonName person) => _items.Count == 1 && _items[0].PersonName == person;

        private string SelectRandomPerson(PersonName excluding = null)
        {
            var names = _items
                .Select(i => i.PersonName)
                .Where(name => excluding == null || name != excluding)
                .ToList();
            
            return names
                .Skip(new Random().Next(names.Count))
                .FirstOrDefault();
        }

        protected override void ApplyEvent(object @event)
        {
            switch(@event)
            {
                case NewOrderStarted e:
                    Id = e.OrderId;
                    _date = e.Date;
                    _localId = e.LocalId;
                    break;

                case OrderItemAdded e:
                    _items.Add(new OrderItem {
                        ItemName = e.ItemName,
                        PersonName = e.PersonName,
                        AddedAt = e.AddedAt
                    });
                    break;
                    
                 case OrderResponsiblePersonSelected e:
                     _responsiblePerson = e.PersonName;
                     break;
                 
                 case OrderResponsiblePersonRemoved e:
                     _responsiblePerson = null;
                     break;
                 
                 case OrderItemRemoved e:
                     _items.Remove(FindItemByPerson(e.PersonName));
                     break;
            }
        }

        private class OrderItem 
        {
            public ItemName ItemName { get; set; }
            public PersonName PersonName { get; set; }
            public DateTime AddedAt { get; set; }
        }
    }
}