using Common.RequestWrapper;
using Domain.Dtos;

namespace Application.Features.Dashboard.Queries
{
    public class GetSalesTrendsQuery : IQuery<SalesTrendDto>
    {
        public int Days { get; set; }

        public GetSalesTrendsQuery()
        {
            Days = 30; // Default to last 30 days
        }

        public GetSalesTrendsQuery(int days)
        {
            Days = days > 0 ? days : 30;
        }
    }
}