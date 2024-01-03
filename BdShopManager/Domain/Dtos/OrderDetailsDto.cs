using Domain.Enums;

namespace Domain.Dtos
{
    public class OrderDetailsDto
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public ProductUnit Unit { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
