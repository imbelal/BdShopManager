using Common.Events;

namespace Domain.Events
{
    public class SalesReturnCreatedEvent : IDomainEvent
    {
        public Guid SalesReturnId { get; private set; }
        public List<SalesReturnItemInfo> SalesReturnItems { get; private set; }

        public SalesReturnCreatedEvent(Guid salesReturnId, List<SalesReturnItemInfo> salesReturnItems)
        {
            SalesReturnId = salesReturnId;
            SalesReturnItems = salesReturnItems;
        }
    }

    public class SalesReturnItemInfo
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }

        public SalesReturnItemInfo(Guid productId, int quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }
    }
}
