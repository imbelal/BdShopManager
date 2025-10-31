namespace Domain.Dtos
{
    public class CreatePurchaseItemDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal CostPerUnit { get; set; }
    }
}
