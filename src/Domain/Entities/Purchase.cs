using Common.Entities;
using Common.Entities.Interfaces;
using Domain.Enums;
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
        public PurchaseStatus Status { get; private set; }
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
            Status = PurchaseStatus.Pending; // Default status
            TotalCost = 0; // Will be calculated when items are added
        }

        // Static factory method to create purchase with items (bulk operation like Sales)
        public static Purchase CreatePurchaseWithItems(Guid supplierId, DateTime purchaseDate, string remark, List<Dtos.CreatePurchaseItemDto> purchaseItemDtos)
        {
            Purchase newPurchase = new(supplierId, purchaseDate, remark);

            // Add all purchase items
            foreach (var item in purchaseItemDtos)
            {
                var purchaseItem = new PurchaseItem(newPurchase.Id, item.ProductId, item.Quantity, item.CostPerUnit);
                newPurchase._purchaseItems.Add(purchaseItem);
            }

            newPurchase.RecalculateTotalCost();

            // Raise domain event with all items at once for bulk processing
            var purchaseItemInfos = purchaseItemDtos.Select(pi => new PurchaseItemInfo(pi.ProductId, pi.Quantity, pi.CostPerUnit)).ToList();
            newPurchase.RaiseDomainEvent(new PurchaseCreatedEvent(newPurchase.Id, purchaseItemInfos));

            return newPurchase;
        }

        // Update purchase header info only
        public void Update(Guid supplierId, DateTime purchaseDate, string remark)
        {
            SupplierId = supplierId;
            PurchaseDate = purchaseDate;
            Remark = remark;
        }

        // Update purchase with items (complete replacement)
        public void UpdateWithItems(Guid supplierId, DateTime purchaseDate, string remark, List<Dtos.CreatePurchaseItemDto> purchaseItemDtos)
        {
            // Capture old purchase items before clearing for stock adjustment
            var oldPurchaseItemInfos = _purchaseItems.Select(pi => new PurchaseItemInfo(pi.ProductId, pi.Quantity, pi.CostPerUnit)).ToList();

            SupplierId = supplierId;
            PurchaseDate = purchaseDate;
            Remark = remark;

            // Clear existing purchase items and add new ones
            _purchaseItems.Clear();
            foreach (var item in purchaseItemDtos)
            {
                var purchaseItem = new PurchaseItem(Id, item.ProductId, item.Quantity, item.CostPerUnit);
                _purchaseItems.Add(purchaseItem);
            }

            RecalculateTotalCost();

            // Raise domain event for stock adjustment and average cost recalculation
            var newPurchaseItemInfos = purchaseItemDtos.Select(pi => new PurchaseItemInfo(pi.ProductId, pi.Quantity, pi.CostPerUnit)).ToList();
            RaiseDomainEvent(new PurchaseUpdatedEvent(Id, oldPurchaseItemInfos, newPurchaseItemInfos));
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

            // Raise domain event to reverse stock and create reversal transactions
            var purchaseItemInfos = _purchaseItems.Select(pi => new PurchaseItemInfo(pi.ProductId, pi.Quantity, pi.CostPerUnit)).ToList();
            RaiseDomainEvent(new PurchaseDeletedEvent(Id, purchaseItemInfos));
        }

        // Cancel purchase - reduces stock and creates cancellation transaction
        public void Cancel()
        {
            if (Status == PurchaseStatus.Cancelled)
                return; // Already cancelled

            Status = PurchaseStatus.Cancelled;

            // Raise domain event to reverse stock and create cancellation transactions
            var purchaseItemInfos = _purchaseItems.Select(pi => new PurchaseItemInfo(pi.ProductId, pi.Quantity, pi.CostPerUnit)).ToList();
            RaiseDomainEvent(new PurchaseCancelledEvent(Id, purchaseItemInfos));
        }

        // Mark purchase as completed
        public void Complete()
        {
            if (Status == PurchaseStatus.Cancelled)
                throw new InvalidOperationException("Cannot complete a cancelled purchase.");

            Status = PurchaseStatus.Completed;
        }
    }
}
