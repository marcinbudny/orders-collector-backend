using OrdersCollector.Domain;

namespace OrdersCollector.Utils
{
    
    public class IdValue : Value<IdValue>
    {
        public string Value { get; }

        protected IdValue(string id)
        {
            ValidateOrThrow(id);

            Value = id;
        }

        public static void ValidateOrThrow(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Id cannot be null or whitespace.", "IdCannotBeEmpty");
        }

        public static implicit operator string(IdValue self) => self.Value;

    }

}