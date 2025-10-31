namespace Domain.Dtos
{
    public class PurchaseItemDto
    {
        public Guid Id { get; set; }
        public Guid PurchaseId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal CostPerUnit { get; set; }
        public decimal TotalCost { get; set; }
    }
}
