using System;

namespace OrdersCollector.Utils
{
    public interface ITimeProvider 
    {
        DateTime UtcNow { get; }
    }

    public class TimeProvider : ITimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }

    public class MockTimeProvider : ITimeProvider
    {
        public DateTime UtcNow { get; set; } = DateTime.UtcNow;
    }
}