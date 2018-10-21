namespace OrdersCollector.Domain.Order
{
    public class ErrorCode
    {
        public const string NotFound = "NotFound";

        public const string IdCannotBeEmpty = "IdCannotBeEmpty";
        public const string ItemCannotBeEmpty = "ItemCannotBeEmpty";
        public const string PersonNameCannotBeEmpty = "PersonNameCannotBeEmpty";
        public const string OrderDateTimePart = "OrderDateTimePart";
        
        public const string PersonAlreadyOrdered = "PersonAlreadyOrdered";
        public const string NooneToSelectFrom = "NooneToSelectFrom";
        public const string ResponsiblePersonAlreadySelected = "ResponsiblePersonAlreadySelected";
        public const string CannotRemoveItemPersonDidntOrder = "CannotRemoveItemPersonDidntOrder";
        
    }

}