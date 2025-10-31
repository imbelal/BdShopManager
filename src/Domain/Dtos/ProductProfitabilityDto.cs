namespace Domain.Dtos
{
    public class ProductProfitabilityDto
    {
        public Guid ProductId { get; set; }
        public string ProductTitle { get; set; }
        public int TotalUnitsSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalProfit { get; set; }
        public decimal AverageProfitMargin { get; set; }
        public decimal CurrentCostPrice { get; set; }
        public decimal CurrentSellingPrice { get; set; }
        public decimal CurrentProfitMargin { get; set; }
    }
}
