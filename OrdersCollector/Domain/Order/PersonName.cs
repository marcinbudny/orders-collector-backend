using OrdersCollector.Utils;

namespace OrdersCollector.Domain.Order
{
    public class PersonName : Value<PersonName>
    {
        public string Value { get; }
        
        public PersonName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Person name cannot be null or whitespace.", ErrorCode.PersonNameCannotBeEmpty);

            Value = value;
        }

        public static implicit operator string(PersonName self) => self.Value;

        public static implicit operator PersonName(string value) => new PersonName(value);
    }
}