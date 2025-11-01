using Common.Entities;
using Common.Entities.Interfaces;
using Domain.Enums;

namespace Domain.Entities
{
    public class StockTransaction : AuditableTenantEntityBase<Guid>, IAggregateRoot, ISoftDeletable
    {
        public Guid ProductId { get; set; }
        public StockTransactionType Type { get; set; }
        public StockReferenceType RefType { get; set; }
        public Guid RefId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; private set; }
        public DateTime TransactionDate { get; set; }
        public bool IsDeleted { get; set; } = false;
        public Tenant Tenant { get; set; }

        public StockTransaction() : base()
        {
        }

        public StockTransaction(
            Guid productId,
            StockTransactionType type,
            StockReferenceType refType,
            Guid refId,
            int quantity,
            decimal unitCost,
            DateTime transactionDate) : base(Guid.NewGuid())
        {
            ProductId = productId;
            Type = type;
            RefType = refType;
            RefId = refId;
            Quantity = quantity;
            UnitCost = unitCost;
            TotalCost = quantity * unitCost;
            TransactionDate = transactionDate;
        }

        public static StockTransaction CreateInbound(
            Guid productId,
            StockReferenceType refType,
            Guid refId,
            int quantity,
            decimal unitCost,
            DateTime transactionDate)
        {
            return new StockTransaction(
                productId,
                StockTransactionType.IN,
                refType,
                refId,
                quantity,
                unitCost,
                transactionDate);
        }

        public static StockTransaction CreateOutbound(
            Guid productId,
            StockReferenceType refType,
            Guid refId,
            int quantity,
            decimal unitCost,
            DateTime transactionDate)
        {
            return new StockTransaction(
                productId,
                StockTransactionType.OUT,
                refType,
                refId,
                quantity,
                unitCost,
                transactionDate);
        }
    }
}
