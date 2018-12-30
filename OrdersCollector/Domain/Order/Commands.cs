namespace OrdersCollector.Domain.Order
{
    namespace Commands.V1
    {
        public class OrderItem
        {
            public string LocalId { get; set; }

            public string ItemName { get; set; }

            public string PersonName { get; set; }

            public string CommandId { get; set; }
        }

        public class SelectResponsiblePerson
        {
            public string OrderId { get; set; }

            public string CommandId { get; set; }
        }

        public class RemoveItem
        {
            public string OrderId { get; set; }

            public string PersonName { get; set; }

            public string CommandId { get; set; }
        }
    }
}