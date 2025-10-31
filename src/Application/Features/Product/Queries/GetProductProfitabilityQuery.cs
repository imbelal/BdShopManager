using Common.Pagination;
using Common.RequestWrapper;
using Domain.Dtos;

namespace Application.Features.Product.Queries
{
    public class GetProductProfitabilityQuery : PaginationQueryBase, IQuery<Pagination<ProductProfitabilityDto>>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? MinimumProfitMargin { get; set; }

        public GetProductProfitabilityQuery(int pageNumber = 1, int pageSize = 10, DateTime? startDate = null, DateTime? endDate = null, decimal? minimumProfitMargin = null)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            StartDate = startDate;
            EndDate = endDate;
            MinimumProfitMargin = minimumProfitMargin;
        }
    }
}
