using System;
using OrdersCollector.Api.Contract;

namespace OrdersCollector.Tests.Integration
{
    public class TestApiCalErrorException : Exception
    {
        public Error Error { get; }

        public TestApiCalErrorException(Error error) => Error = error;
    }
}