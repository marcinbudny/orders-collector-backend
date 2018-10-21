using System;
using System.Collections.Generic;
using System.Linq;
using OrdersCollector.Domain.Order.Events.V1;

namespace OrdersCollector.Reads.Document
{
    public class Order 
    {
        public string Id { get; set; }

        public DateTime Date { get; set; }

        public string LocalId { get; set; }

        public string ResponsiblePerson { get; set; }

        public List<OrderItem> Items { get; set; } = new List<OrderItem>();

        public void Apply(NewOrderStarted e)
        {
            Id = e.OrderId;
            Date = e.Date;
            LocalId = e.LocalId;
        }

        public void Apply(OrderItemAdded e)
        {
            if(Items.Any(i => i.PersonName == e.PersonName))
                return;
            
            Items.Add(new OrderItem
            {
                PersonName = e.PersonName,
                ItemName = e.ItemName,
                AddedAt = e.AddedAt
            });
        }

        public void Apply(OrderResponsiblePersonSelected e)
        {
            ResponsiblePerson = e.PersonName;
        }

        public void Apply(OrderResponsiblePersonRemoved e)
        {
            ResponsiblePerson = null;
        }

        public void Apply(OrderItemRemoved e)
        {
            var item = Items.FirstOrDefault(i => i.PersonName == e.PersonName);
            if (item != null)
                Items.Remove(item);
        }
    }

    public class OrderItem
    {
        public string PersonName { get; set; }

        public string ItemName { get; set; }

        public DateTime AddedAt { get; set; }
    }
}