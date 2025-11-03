using Common.Pagination;
using Common.RequestWrapper;
using Domain.Dtos;

namespace Application.Features.Product.Queries
{
    public class GetAllProductsQuery : PaginationQueryBase, IQuery<Pagination<ProductDto>>
    {
        public string? SearchTerm { get; set; }
        public Guid? CategoryId { get; set; }
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }
        public int? Unit { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? InStock { get; set; }

        public GetAllProductsQuery(
            int pageNumber = 1,
            int pageSize = 10,
            string searchTerm = null,
            Guid? categoryId = null,
            string sortBy = null,
            string sortOrder = null,
            int? unit = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            bool? inStock = null)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            SearchTerm = searchTerm;
            CategoryId = categoryId;
            SortBy = sortBy;
            SortOrder = sortOrder;
            Unit = unit;
            MinPrice = minPrice;
            MaxPrice = maxPrice;
            InStock = inStock;
        }
    }
}
