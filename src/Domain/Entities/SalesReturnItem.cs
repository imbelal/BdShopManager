using Common.Entities;
using Domain.Enums;

namespace Domain.Entities
{
    public class SalesReturnItem : AuditableTenantEntityBase<Guid>
    {
        public Guid SalesReturnId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public ProductUnit Unit { get; set; }
        public decimal UnitPrice { get; set; } // Price at which it was sold
        public decimal TotalPrice { get; set; }
        public string Reason { get; set; }

        public virtual SalesReturn SalesReturn { get; set; }
        public Tenant Tenant { get; set; }

        public SalesReturnItem() : base()
        {

        }

        public SalesReturnItem(Guid salesReturnId, Guid productId, int quantity, ProductUnit unit, decimal unitPrice, string reason) : base(Guid.NewGuid())
        {
            SalesReturnId = salesReturnId;
            ProductId = productId;
            Quantity = quantity;
            Unit = unit;
            UnitPrice = unitPrice;
            TotalPrice = unitPrice * quantity;
            Reason = reason;
        }
    }
}
