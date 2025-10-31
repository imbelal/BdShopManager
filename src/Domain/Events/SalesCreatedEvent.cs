using Common.Events;

namespace Domain.Events
{
    public class SalesCreatedEvent : IDomainEvent
    {
        public Guid SalesId { get; private set; }
        public List<SalesItemInfo> SalesItems { get; private set; }

        public SalesCreatedEvent(Guid salesId, List<SalesItemInfo> salesItems)
        {
            SalesId = salesId;
            SalesItems = salesItems;
        }
    }

    public class SalesItemInfo
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }

        public SalesItemInfo(Guid productId, int quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }
    }
}
