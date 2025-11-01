using Common.Events;
using Domain.Enums;

namespace Domain.Events
{
    public class StockAdjustmentCreatedEvent : IDomainEvent
    {
        public Guid AdjustmentId { get; private set; }
        public Guid ProductId { get; private set; }
        public StockTransactionType Type { get; private set; }
        public int Quantity { get; private set; }

        public StockAdjustmentCreatedEvent(
            Guid adjustmentId,
            Guid productId,
            StockTransactionType type,
            int quantity)
        {
            AdjustmentId = adjustmentId;
            ProductId = productId;
            Type = type;
            Quantity = quantity;
        }
    }
}
