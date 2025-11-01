using Common.Events;

namespace Domain.Events
{
    public class PurchaseCreatedEvent : IDomainEvent
    {
        public Guid PurchaseId { get; private set; }
        public List<PurchaseItemInfo> PurchaseItems { get; private set; }

        public PurchaseCreatedEvent(Guid purchaseId, List<PurchaseItemInfo> purchaseItems)
        {
            PurchaseId = purchaseId;
            PurchaseItems = purchaseItems;
        }
    }
}
