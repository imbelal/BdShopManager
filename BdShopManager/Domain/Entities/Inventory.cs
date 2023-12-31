using Common.Entities;
using Common.Entities.Interfaces;

namespace Domain.Entities
{
    public class Inventory : AuditableEntityBase, IAggregateRoot, ISoftDeletable
    {
        public Guid ProductId { get; set; }
        public Guid SupplierId { get; set; }
        public int Quantity { get; set; }
        public decimal CostPerUnit { get; set; }
        public decimal TotalCost { get; set; }
        public string Remark { get; set; }
        public bool IsDeleted { get; set; } = false;

        public Inventory(Guid productId, Guid supplierId, int quantity, decimal costPerUnit, decimal totalCost, string remark)
        {
            ProductId = productId;
            SupplierId = supplierId;
            Quantity = quantity;
            CostPerUnit = costPerUnit;
            TotalCost = totalCost;
            Remark = remark;
        }
    }
}
