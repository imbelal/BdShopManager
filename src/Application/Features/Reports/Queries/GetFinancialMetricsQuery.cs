using Common.RequestWrapper;
using Domain.Dtos;

namespace Application.Features.Reports.Queries
{
    public class GetFinancialMetricsQuery : IQuery<FinancialMetricsDto>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IncludeKPIs { get; set; } = true;
        public bool IncludeRatios { get; set; } = true;
        public bool IncludeCashFlow { get; set; } = true;
        public string Currency { get; set; } = "USD";

        public GetFinancialMetricsQuery()
        {
        }

        public GetFinancialMetricsQuery(DateTime startDate, DateTime endDate,
            bool includeKPIs = true, bool includeRatios = true, bool includeCashFlow = true)
        {
            StartDate = startDate;
            EndDate = endDate;
            IncludeKPIs = includeKPIs;
            IncludeRatios = includeRatios;
            IncludeCashFlow = includeCashFlow;
        }
    }
}