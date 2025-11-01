using Domain.Dtos;
using Domain.Enums;

namespace Domain.Dtos
{
    public class UpdateSalesReturnDto
    {
        public decimal TotalRefundAmount { get; set; }
        public string Remark { get; set; }
        public List<UpdateSalesReturnItemDto> SalesReturnItems { get; set; } = new();
    }

    public class UpdateSalesReturnItemDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public ProductUnit Unit { get; set; }
        public decimal UnitPrice { get; set; }
        public string Reason { get; set; }
    }
}