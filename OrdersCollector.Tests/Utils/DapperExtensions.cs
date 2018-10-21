using System.Collections.Generic;
using System.Linq;

namespace OrdersCollector.Tests.Utils
{
    public static class DapperExtensions
    {
        public static IEnumerable<IDictionary<string, object>> ToDictionaries(this IEnumerable<dynamic> subject)
            => subject.Cast<IDictionary<string, object>>();
        
        public static void DapperReadModelShouldBeEquivalentTo(this IEnumerable<dynamic> subject, params IDictionary<string, object>[] dicts) 
            => FluentAssertions.AssertionExtensions.Should(subject.ToDictionaries()).BeEquivalentTo(dicts);
    }
}