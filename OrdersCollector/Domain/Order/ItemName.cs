using System.Data;
using OrdersCollector.Utils;

namespace OrdersCollector.Domain.Order
{
    public class ItemName : Value<ItemName>
    {
        public string Value { get; }
        
        public ItemName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Item name cannot be null or whitespace.", ErrorCode.ItemCannotBeEmpty);

            Value = value;
        }

        public static implicit operator string(ItemName self) => self.Value;

        public static implicit operator ItemName(string value) => new ItemName(value);
    }
}