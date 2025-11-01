namespace Domain.Dtos
{
    public class SalesReturnDto
    {
        public Guid Id { get; set; }
        public string ReturnNumber { get; set; }
        public Guid SalesId { get; set; }
        public string SalesNumber { get; set; }
        public decimal TotalRefundAmount { get; set; }
        public string Remark { get; set; }
        public DateTime CreatedUtcDate { get; set; }
        public List<SalesReturnItemDto> SalesReturnItems { get; set; } = new();
    }
}
