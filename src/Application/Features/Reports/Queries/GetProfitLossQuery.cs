using Common.RequestWrapper;
using Domain.Dtos;

namespace Application.Features.Reports.Queries
{
    public class GetProfitLossQuery : IQuery<ProfitLossDto>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string GroupBy { get; set; } = "month"; // "day", "month", "year"
        public bool IncludeComparisons { get; set; } = true;
        public string Currency { get; set; } = "USD";

        public GetProfitLossQuery()
        {
        }

        public GetProfitLossQuery(DateTime startDate, DateTime endDate, string groupBy = "month", bool includeComparisons = true)
        {
            StartDate = startDate;
            EndDate = endDate;
            GroupBy = groupBy;
            IncludeComparisons = includeComparisons;
        }
    }
}