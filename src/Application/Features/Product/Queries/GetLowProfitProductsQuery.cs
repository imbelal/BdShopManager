using Common.Pagination;
using Common.RequestWrapper;
using Domain.Dtos;

namespace Application.Features.Product.Queries
{
    public class GetLowProfitProductsQuery : PaginationQueryBase, IQuery<Pagination<ProductDto>>
    {
        public decimal MaxProfitMargin { get; set; }

        public GetLowProfitProductsQuery(decimal maxProfitMargin = 10.0m, int pageNumber = 1, int pageSize = 10)
        {
            MaxProfitMargin = maxProfitMargin;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
