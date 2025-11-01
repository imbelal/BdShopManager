using Common.Events;

namespace Domain.Events
{
    public class PurchaseDeletedEvent : IDomainEvent
    {
        public Guid PurchaseId { get; private set; }
        public List<PurchaseItemInfo> PurchaseItems { get; private set; }

        public PurchaseDeletedEvent(Guid purchaseId, List<PurchaseItemInfo> purchaseItems)
        {
            PurchaseId = purchaseId;
            PurchaseItems = purchaseItems;
        }
    }

    public class PurchaseItemInfo
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal CostPerUnit { get; set; }

        public PurchaseItemInfo(Guid productId, int quantity, decimal costPerUnit)
        {
            ProductId = productId;
            Quantity = quantity;
            CostPerUnit = costPerUnit;
        }
    }
}
