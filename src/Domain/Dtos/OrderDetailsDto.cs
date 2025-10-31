using Domain.Enums;

namespace Domain.Dtos
{
    public class OrderDetailsDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public ProductUnit Unit { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal UnitCost { get; set; } // Cost price at time of order (populated by command handler)
    }
}
