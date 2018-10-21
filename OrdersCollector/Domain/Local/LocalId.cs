using OrdersCollector.Utils;

namespace OrdersCollector.Domain.Local
{
    public class LocalId : IdValue
    {
        public LocalId(string value) : base(value) { }

        public static implicit operator string(LocalId self) => self.Value;

        public static implicit operator LocalId(string value) => new LocalId(value);
    }
}