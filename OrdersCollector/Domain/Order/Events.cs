using System;

namespace OrdersCollector.Domain.Order
{
    namespace Events.V1
    {
        public class NewOrderStarted : EventMetadata
        {
            public string OrderId { get; }

            public DateTime Date { get; }

            public string LocalId { get; }

            public NewOrderStarted(string orderId, string localId, DateTime date)
            {
                OrderId = orderId;
                Date = date;
                LocalId = localId;
            }
        }

        public class OrderItemAdded : EventMetadata
        {
            public string OrderId { get; }

            public string ItemName { get; }

            public string PersonName { get; }

            public DateTime AddedAt { get; }

            public OrderItemAdded(string orderId, string itemName, string personName, DateTime addedAt)
            {
                OrderId = orderId;
                ItemName = itemName;
                PersonName = personName;
                AddedAt = addedAt;
            }
        }

        public class OrderResponsiblePersonSelected : EventMetadata
        {
            public string OrderId { get; }

            public string PersonName { get; }

            public OrderResponsiblePersonSelected(string orderId, string personName)
            {
                OrderId = orderId;
                PersonName = personName;
            }
        }

        public class OrderItemRemoved : EventMetadata
        {
            public string OrderId { get; }

            public string ItemName { get; }

            public string PersonName { get; }

            public OrderItemRemoved(string orderId, string itemName, string personName)
            {
                OrderId = orderId;
                ItemName = itemName;
                PersonName = personName;
            }
        }

        public class OrderResponsiblePersonRemoved : EventMetadata
        {
            public string OrderId { get; }

            public OrderResponsiblePersonRemoved(string orderId)
            {
                OrderId = orderId;
            }
        }
    }
}