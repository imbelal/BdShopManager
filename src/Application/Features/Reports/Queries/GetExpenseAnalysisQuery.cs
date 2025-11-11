using Common.RequestWrapper;
using Domain.Dtos;

namespace Application.Features.Reports.Queries
{
    public class GetExpenseAnalysisQuery : IQuery<ExpenseAnalysisDto>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string GroupBy { get; set; } = "month"; // "day", "month", "year"
        public bool IncludeVendorBreakdown { get; set; } = true;
        public bool IncludeBudgetComparison { get; set; } = true;
        public int TopVendorsCount { get; set; } = 10;
        public string Currency { get; set; } = "USD";

        public GetExpenseAnalysisQuery()
        {
        }

        public GetExpenseAnalysisQuery(DateTime startDate, DateTime endDate, string groupBy = "month",
            bool includeVendorBreakdown = true, bool includeBudgetComparison = true, int topVendorsCount = 10)
        {
            StartDate = startDate;
            EndDate = endDate;
            GroupBy = groupBy;
            IncludeVendorBreakdown = includeVendorBreakdown;
            IncludeBudgetComparison = includeBudgetComparison;
            TopVendorsCount = topVendorsCount;
        }
    }
}