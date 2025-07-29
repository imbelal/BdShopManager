using Common.Entities;
using Common.Entities.Interfaces;
using Domain.Events;

namespace Domain.Entities
{
    public class Inventory : AuditableTenantEntityBase<Guid>, IAggregateRoot, ISoftDeletable
    {
        public Guid ProductId { get; private set; }
        public Guid SupplierId { get; private set; }
        public int Quantity { get; private set; }
        public decimal CostPerUnit { get; private set; }
        public decimal TotalCost { get; private set; }
        public string Remark { get; private set; }
        public bool IsDeleted { get; set; } = false;
        public Tenant Tenant { get; set; }

        public Inventory() : base()
        {

        }

        public Inventory(Guid productId, Guid supplierId, int quantity, decimal costPerUnit, decimal totalCost, string remark) : base(Guid.NewGuid())
        {
            ProductId = productId;
            SupplierId = supplierId;
            Quantity = quantity;
            CostPerUnit = costPerUnit;
            TotalCost = totalCost;
            Remark = remark;

            // Raise domain event for inventory added
            RaiseDomainEvent(new InventoryAddedEvent(productId, quantity));
        }

        public void DecreaseQuantity(int quantity)
        {
            if (quantity > 0)
            {
                Quantity -= quantity;
            }
        }

    }
}
