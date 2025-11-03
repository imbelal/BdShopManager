namespace Domain.Dtos
{
    public class CustomerDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public string Address { get; set; }
        public string Remark { get; set; }
        public decimal TotalDueAmount { get; set; }
        public int TotalSales { get; set; }
        public decimal TotalSalesAmount { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public DateTime? LastSaleDate { get; set; }
        public string FormattedLastSaleDate { get; set; }
        public string FormattedTotalDueAmount { get; set; }
        public string FormattedTotalSalesAmount { get; set; }
    }
}