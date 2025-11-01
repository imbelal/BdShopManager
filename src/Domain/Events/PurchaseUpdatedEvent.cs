using Common.Events;

namespace Domain.Events
{
    public class PurchaseUpdatedEvent : IDomainEvent
    {
        public Guid PurchaseId { get; private set; }
        public List<PurchaseItemInfo> OldPurchaseItems { get; private set; }
        public List<PurchaseItemInfo> NewPurchaseItems { get; private set; }

        public PurchaseUpdatedEvent(Guid purchaseId, List<PurchaseItemInfo> oldPurchaseItems, List<PurchaseItemInfo> newPurchaseItems)
        {
            PurchaseId = purchaseId;
            OldPurchaseItems = oldPurchaseItems;
            NewPurchaseItems = newPurchaseItems;
        }
    }
}
