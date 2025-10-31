using Common.Entities;
using Common.Entities.Interfaces;
using Domain.Events;

namespace Domain.Entities
{
    public class Purchase : AuditableTenantEntityBase<Guid>, IAggregateRoot, ISoftDeletable
    {
        private readonly List<PurchaseItem> _purchaseItems = new();

        public Guid SupplierId { get; private set; }
        public DateTime PurchaseDate { get; private set; }
        public decimal TotalCost { get; private set; }
        public string Remark { get; private set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public IReadOnlyCollection<PurchaseItem> PurchaseItems => _purchaseItems.AsReadOnly();
        public Tenant Tenant { get; set; }

        // EF Core constructor
        private Purchase() : base()
        {
        }

        // Domain constructor
        public Purchase(Guid supplierId, DateTime purchaseDate, string remark) : base(Guid.NewGuid())
        {
            SupplierId = supplierId;
            PurchaseDate = purchaseDate;
            Remark = remark;
            TotalCost = 0; // Will be calculated when items are added
        }

        // Add purchase item and recalculate total cost
        public void AddPurchaseItem(Guid productId, int quantity, decimal costPerUnit)
        {
            var purchaseItem = new PurchaseItem(Id, productId, quantity, costPerUnit);
            _purchaseItems.Add(purchaseItem);
            RecalculateTotalCost();

            // Raise domain event when purchase item is added
            // This will trigger updating product stock and average cost
            RaiseDomainEvent(new PurchaseCreatedEvent(Id, productId, quantity, costPerUnit));
        }

        // Update purchase header info
        public void Update(Guid supplierId, DateTime purchaseDate, string remark)
        {
            SupplierId = supplierId;
            PurchaseDate = purchaseDate;
            Remark = remark;
        }

        // Remove purchase item and recalculate total
        public void RemovePurchaseItem(Guid purchaseItemId)
        {
            var item = _purchaseItems.FirstOrDefault(i => i.Id == purchaseItemId);
            if (item != null)
            {
                _purchaseItems.Remove(item);
                RecalculateTotalCost();
            }
        }

        // Update purchase item and recalculate total
        public void UpdatePurchaseItem(Guid purchaseItemId, Guid productId, int quantity, decimal costPerUnit)
        {
            var item = _purchaseItems.FirstOrDefault(i => i.Id == purchaseItemId);
            if (item != null)
            {
                item.Update(productId, quantity, costPerUnit);
                RecalculateTotalCost();
            }
        }

        // Business logic: Recalculate total cost based on all purchase items
        private void RecalculateTotalCost()
        {
            TotalCost = _purchaseItems.Sum(i => i.TotalCost);
        }

        public void Delete()
        {
            IsDeleted = true;
            foreach (var item in _purchaseItems)
            {
                item.Delete();
            }
        }
    }
}
