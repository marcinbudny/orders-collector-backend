using System;

namespace OrdersCollector.Domain
{
    public class DomainException : Exception
    {
        public string ErrorCode { get; private set; }
        
        public DomainException(string message, string errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}