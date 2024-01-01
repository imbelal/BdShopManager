using Common.Entities;
using Common.Entities.Interfaces;

namespace Domain.Entities
{
    public class Inventory : AuditableEntityBase, IAggregateRoot, ISoftDeletable
    {
        public Guid ProductId { get; private set; }
        public Guid SupplierId { get; private set; }
        public int Quantity { get; private set; }
        public decimal CostPerUnit { get; private set; }
        public decimal TotalCost { get; private set; }
        public string Remark { get; private set; }
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
