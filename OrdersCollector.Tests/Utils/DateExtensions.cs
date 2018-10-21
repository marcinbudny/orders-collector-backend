using System;

namespace OrdersCollector.Tests.Utils
{
    public static class DateExtensions
    {
        public static DateTime ToDateTime(this string text) => DateTime.Parse(text);
       
        public static DateTime ToUtcDateTime(this string text)
        {
            var dateTime = DateTime.Parse(text);
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }
    }
}