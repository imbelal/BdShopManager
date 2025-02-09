using Common.Pagination;
using Common.RequestWrapper;
using Domain.Dtos;

namespace Application.Features.Product.Queries
{
    public class GetAllProductsQuery : PaginationQueryBase, IQuery<Pagination<ProductDto>>
    {
        public GetAllProductsQuery(int pageNumber = 1, int pageSize = 10)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
