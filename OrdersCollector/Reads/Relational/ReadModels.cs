using System;

namespace OrdersCollector.Reads.Relational
{
    public class Order 
    {
        public string Id { get; set; }

        public string Date { get; set; }

        public string LocalId { get; set; }
    }

    public class OrderItem
    {
        public string PersonName { get; set; }

        public string ItemName { get; set; }

        public DateTime AddedAt { get; set; }

        public string OrderId { get; set; }
    }
}