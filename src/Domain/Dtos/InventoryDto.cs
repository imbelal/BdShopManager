namespace Domain.Dtos
{
    public class InventoryDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public Guid SupplierId { get; set; }
        public string SupplierName { get; set; }
        public int Quantity { get; set; }
        public decimal CostPerUnit { get; set; }
        public decimal TotalCost { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
    }
}
