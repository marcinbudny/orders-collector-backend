using System;
using System.Collections.Generic;
using OrdersCollector.Utils;

namespace OrdersCollector.Domain.Order
{
    public class OrderDate : Value<OrderDate>
    {
        public DateTime Value { get; }
        
        public OrderDate(DateTime value) 
        {
            if(value - value.Date != TimeSpan.Zero)
                throw new DomainException("The order date cannot contain time part", ErrorCode.OrderDateTimePart);
            
            Value = value;
        }

        public static OrderDate Today(ITimeProvider timeProvider) => new OrderDate(timeProvider.UtcNow.Date);

        public static OrderDate From(DateTime dateTime) => new OrderDate(dateTime.Date);

        public static implicit operator DateTime(OrderDate self) => self.Value;

        public static implicit operator OrderDate(DateTime value) => new OrderDate(value);

        public override string ToString() => Value.ToString("yyyy-MM-dd"); 
    }
}