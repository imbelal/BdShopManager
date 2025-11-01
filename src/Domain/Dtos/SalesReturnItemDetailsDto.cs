using Domain.Enums;

namespace Domain.Dtos
{
    public class SalesReturnItemDetailsDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public ProductUnit Unit { get; set; }
        public decimal UnitPrice { get; set; }
        public string Reason { get; set; }
    }
}
