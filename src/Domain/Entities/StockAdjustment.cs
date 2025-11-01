using Common.Entities;
using Common.Entities.Interfaces;
using Domain.Enums;
using Domain.Events;

namespace Domain.Entities
{
    public class StockAdjustment : AuditableTenantEntityBase<Guid>, IAggregateRoot, ISoftDeletable
    {
        public Guid ProductId { get; private set; }
        public StockTransactionType Type { get; private set; }
        public int Quantity { get; private set; }
        public string Reason { get; private set; }
        public DateTime AdjustmentDate { get; private set; }
        public bool IsDeleted { get; set; } = false;
        public Tenant Tenant { get; set; }

        // EF Core constructor
        private StockAdjustment() : base()
        {
        }

        // Domain constructor
        private StockAdjustment(
            Guid productId,
            StockTransactionType type,
            int quantity,
            string reason,
            DateTime adjustmentDate) : base(Guid.NewGuid())
        {
            ProductId = productId;
            Type = type;
            Quantity = quantity;
            Reason = reason;
            AdjustmentDate = adjustmentDate;
        }

        // Factory method to create stock adjustment and raise domain event
        public static StockAdjustment Create(
            Guid productId,
            StockTransactionType type,
            int quantity,
            string reason)
        {
            var adjustment = new StockAdjustment(
                productId,
                type,
                quantity,
                reason,
                DateTime.UtcNow);

            // Raise domain event to update product stock and create transaction record
            adjustment.RaiseDomainEvent(new StockAdjustmentCreatedEvent(
                adjustment.Id,
                productId,
                type,
                quantity));

            return adjustment;
        }
    }
}
