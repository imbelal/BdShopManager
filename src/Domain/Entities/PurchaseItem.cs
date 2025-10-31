using Common.Entities;
using Common.Entities.Interfaces;

namespace Domain.Entities
{
    public class PurchaseItem : AuditableTenantEntityBase<Guid>, ISoftDeletable
    {
        public Guid PurchaseId { get; private set; }
        public Guid ProductId { get; private set; }
        public int Quantity { get; private set; }
        public decimal CostPerUnit { get; private set; }
        public decimal TotalCost { get; private set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public Purchase Purchase { get; private set; }
        public Tenant Tenant { get; set; }

        // EF Core constructor
        private PurchaseItem() : base()
        {
        }

        // Domain constructor - called by Purchase aggregate
        internal PurchaseItem(Guid purchaseId, Guid productId, int quantity, decimal costPerUnit) : base(Guid.NewGuid())
        {
            PurchaseId = purchaseId;
            ProductId = productId;
            Quantity = quantity;
            CostPerUnit = costPerUnit;
            TotalCost = quantity * costPerUnit; // Business logic: calculate total cost
        }

        // Update method - can only be called through Purchase aggregate
        internal void Update(Guid productId, int quantity, decimal costPerUnit)
        {
            ProductId = productId;
            Quantity = quantity;
            CostPerUnit = costPerUnit;
            TotalCost = quantity * costPerUnit;
        }

        public void Delete()
        {
            IsDeleted = true;
        }
    }
}
