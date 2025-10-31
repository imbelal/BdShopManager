using Common.Events;

namespace Domain.Events
{
    public class InventoryAddedEvent : IDomainEvent
    {
        public Guid ProductId { get; private set; }
        public int Quantity { get; private set; }
        public decimal CostPerUnit { get; private set; }

        public InventoryAddedEvent(Guid productId, int quantity, decimal costPerUnit)
        {
            ProductId = productId;
            Quantity = quantity;
            CostPerUnit = costPerUnit;
        }
    }
}
