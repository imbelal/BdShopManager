using Common.RequestWrapper;
using Domain.Dtos;

namespace Application.Features.Dashboard.Queries
{
    public class GetDashboardMetricsQuery : IQuery<DashboardMetricsDto>
    {
        // Optional date range parameters
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // If no dates provided, will default to current month
        public GetDashboardMetricsQuery()
        {
            var now = DateTime.UtcNow;
            StartDate = new DateTime(now.Year, now.Month, 1);
            EndDate = now;
        }

        public GetDashboardMetricsQuery(DateTime? startDate, DateTime? endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}