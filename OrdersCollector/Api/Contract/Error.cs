using OrdersCollector.Domain;

namespace OrdersCollector.Api.Contract
{
    public class Error
    {
        public static readonly Error InternalServerError = new Error{ Type = "ServerError", Description = "Internal server error"};
        
        public string Type { get; set; }
        public string ErrorCode { get; set; }
        public string Description { get; set; }

        public static Error From(DomainException e) =>
            new Error {Type = "DomainError", ErrorCode = e.ErrorCode, Description = e.Message};
    }
}