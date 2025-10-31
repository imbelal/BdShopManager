using Common.Entities;
using Domain.Enums;

namespace Domain.Entities
{
    public class SalesItem : AuditableTenantEntityBase<Guid>
    {
        public Guid SalesId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public ProductUnit Unit { get; set; }
        public decimal UnitPrice { get; set; } // Selling price at time of order
        public decimal UnitCost { get; set; } // Cost price at time of order
        public decimal TotalPrice { get; set; }
        public decimal TotalCost { get; set; }

        // Calculated properties for item-level profit
        public decimal ItemProfit => TotalPrice - TotalCost;
        public decimal ItemProfitMargin => TotalPrice > 0
            ? Math.Round((ItemProfit / TotalPrice) * 100, 2)
            : 0;

        public virtual Sales Sales { get; set; }
        public Tenant Tenant { get; set; }

        public SalesItem() : base()
        {

        }

        public SalesItem(Guid salesId, Guid productId, int quantity, ProductUnit unit, decimal unitPrice, decimal unitCost) : base(Guid.NewGuid())
        {
            SalesId = salesId;
            ProductId = productId;
            Quantity = quantity;
            Unit = unit;
            UnitPrice = unitPrice;
            UnitCost = unitCost;
            TotalPrice = unitPrice * quantity;
            TotalCost = unitCost * quantity;
        }
    }
}
