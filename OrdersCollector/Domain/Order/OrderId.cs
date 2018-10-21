using OrdersCollector.Utils;

namespace OrdersCollector.Domain.Order
{
    public class OrderId : IdValue
    {
        public OrderId(string id) : base(id) { }

        public static OrderId From(string localId, OrderDate date)
        {
            if(string.IsNullOrWhiteSpace(localId))
                throw new DomainException("Local id cannot be empty", ErrorCode.IdCannotBeEmpty);
            return new OrderId($"{localId}-{date.Value:yyyyMMdd}");
        }

        public static implicit operator OrderId(string value) => new OrderId(value);

    }
}