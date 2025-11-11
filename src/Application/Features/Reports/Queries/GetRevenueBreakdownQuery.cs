using Common.RequestWrapper;
using Domain.Dtos;

namespace Application.Features.Reports.Queries
{
    public class GetRevenueBreakdownQuery : IQuery<FinancialRevenueBreakdownDto>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string GroupBy { get; set; } = "month"; // "day", "month", "year"
        public bool IncludeTopProducts { get; set; } = true;
        public bool IncludeCustomerBreakdown { get; set; } = true;
        public int TopProductsCount { get; set; } = 10;
        public int TopCustomersCount { get; set; } = 10;
        public string Currency { get; set; } = "USD";

        public GetRevenueBreakdownQuery()
        {
        }

        public GetRevenueBreakdownQuery(DateTime startDate, DateTime endDate, string groupBy = "month",
            bool includeTopProducts = true, bool includeCustomerBreakdown = true, int topProductsCount = 10, int topCustomersCount = 10)
        {
            StartDate = startDate;
            EndDate = endDate;
            GroupBy = groupBy;
            IncludeTopProducts = includeTopProducts;
            IncludeCustomerBreakdown = includeCustomerBreakdown;
            TopProductsCount = topProductsCount;
            TopCustomersCount = topCustomersCount;
        }
    }
}