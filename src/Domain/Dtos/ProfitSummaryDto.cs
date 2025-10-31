namespace Domain.Dtos
{
    public class ProfitSummaryDto
    {
        public DateTime Date { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalProfit { get; set; }
        public decimal AverageProfitMargin { get; set; }
    }
}
