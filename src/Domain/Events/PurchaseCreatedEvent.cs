using Common.Events;

namespace Domain.Events
{
    public class PurchaseCreatedEvent : IDomainEvent
    {
        public Guid PurchaseId { get; private set; }
        public Guid ProductId { get; private set; }
        public int Quantity { get; private set; }
        public decimal CostPerUnit { get; private set; }

        public PurchaseCreatedEvent(Guid purchaseId, Guid productId, int quantity, decimal costPerUnit)
        {
            PurchaseId = purchaseId;
            ProductId = productId;
            Quantity = quantity;
            CostPerUnit = costPerUnit;
        }
    }
}
