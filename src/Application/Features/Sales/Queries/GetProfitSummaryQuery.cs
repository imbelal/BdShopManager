using Common.RequestWrapper;
using Domain.Dtos;

namespace Application.Features.Sales.Queries
{
    public class GetProfitSummaryQuery : IQuery<List<ProfitSummaryDto>>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string GroupBy { get; set; } // "day", "month", "year"

        public GetProfitSummaryQuery(DateTime startDate, DateTime endDate, string groupBy = "day")
        {
            StartDate = startDate;
            EndDate = endDate;
            GroupBy = groupBy;
        }
    }
}
