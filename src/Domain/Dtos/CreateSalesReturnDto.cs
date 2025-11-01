namespace Domain.Dtos
{
    public class CreateSalesReturnDto
    {
        public Guid SalesId { get; set; }
        public decimal TotalRefundAmount { get; set; }
        public string Remark { get; set; }
        public List<SalesReturnItemDetailsDto> SalesReturnItems { get; set; }
    }
}
