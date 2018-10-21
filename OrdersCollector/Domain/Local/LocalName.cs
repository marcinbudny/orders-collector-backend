using OrdersCollector.Utils;

namespace OrdersCollector.Domain.Local
{
    public class LocalName : Value<LocalName>
    {
        public string Value { get; }
        
        public LocalName(string value)
        {
            ValidateOrThrow(value);

            Value = value;
        }

        public static void ValidateOrThrow(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Local name cannot be null or whitespace.", ErrorCode.LocalNameCannotBeEmpty);
        }

        public static implicit operator string(LocalName self) => self.Value;

        public static implicit operator LocalName(string value) => new LocalName(value);
    }
}