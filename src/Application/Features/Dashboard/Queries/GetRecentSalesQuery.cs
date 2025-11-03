using Common.Pagination;
using Common.RequestWrapper;
using Domain.Dtos;

namespace Application.Features.Dashboard.Queries
{
    public class GetRecentSalesQuery : PaginationQueryBase, IQuery<Pagination<RecentSaleDto>>
    {
        public int Limit { get; set; } = 10;

        public GetRecentSalesQuery()
        {
            PageNumber = 1;
            PageSize = Limit;
        }

        public GetRecentSalesQuery(int limit)
        {
            PageNumber = 1;
            PageSize = limit > 0 ? limit : 10;
        }
    }
}