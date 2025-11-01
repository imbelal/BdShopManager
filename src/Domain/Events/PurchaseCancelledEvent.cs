using Common.Events;

namespace Domain.Events
{
    public class PurchaseCancelledEvent : IDomainEvent
    {
        public Guid PurchaseId { get; private set; }
        public List<PurchaseItemInfo> PurchaseItems { get; private set; }

        public PurchaseCancelledEvent(Guid purchaseId, List<PurchaseItemInfo> purchaseItems)
        {
            PurchaseId = purchaseId;
            PurchaseItems = purchaseItems;
        }
    }
}