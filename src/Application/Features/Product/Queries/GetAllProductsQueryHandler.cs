using Common.Pagination;
using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Dtos;
using Domain.Interfaces;

namespace Application.Features.Product.Queries
{
    public class GetAllProductsQueryHandler : IQueryHandler<GetAllProductsQuery, Pagination<ProductDto>>
    {
        private readonly IReadOnlyApplicationDbContext _context;
        public GetAllProductsQueryHandler(IReadOnlyApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IResponse<Pagination<ProductDto>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var term = request.SearchTerm?.Trim().ToLower();

            var queryable = _context.Products
                .AsQueryable();

            // Add search filter only if searchTerm is provided
            if (!string.IsNullOrEmpty(term))
            {
                queryable = queryable.Where(p =>
                    p.Title.ToLower().Contains(term) ||
                    p.Description.ToLower().Contains(term) ||
                    (p.Size != null && p.Size.ToLower().Contains(term)) ||
                    (p.Color != null && p.Color.ToLower().Contains(term)));
            }

            // Add category filter
            if (request.CategoryId.HasValue)
            {
                queryable = queryable.Where(p => p.CategoryId == request.CategoryId.Value);
            }

            // Add unit filter
            if (request.Unit.HasValue)
            {
                queryable = queryable.Where(p => (int)p.Unit == request.Unit.Value);
            }

            // Add price range filter
            if (request.MinPrice.HasValue)
            {
                queryable = queryable.Where(p => p.SellingPrice >= request.MinPrice.Value);
            }

            if (request.MaxPrice.HasValue)
            {
                queryable = queryable.Where(p => p.SellingPrice <= request.MaxPrice.Value);
            }

            // Add stock filter
            if (request.InStock.HasValue)
            {
                if (request.InStock.Value)
                {
                    queryable = queryable.Where(p => p.StockQuantity > 0);
                }
                else
                {
                    queryable = queryable.Where(p => p.StockQuantity <= 0);
                }
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(request.SortBy))
            {
                var sortBy = request.SortBy.ToLower();
                var isAscending = request.SortOrder?.ToLower() != "desc";

                switch (sortBy)
                {
                    case "title":
                        queryable = isAscending
                            ? queryable.OrderBy(p => p.Title)
                            : queryable.OrderByDescending(p => p.Title);
                        break;
                    case "price":
                        queryable = isAscending
                            ? queryable.OrderBy(p => p.SellingPrice)
                            : queryable.OrderByDescending(p => p.SellingPrice);
                        break;
                    case "stockquantity":
                        queryable = isAscending
                            ? queryable.OrderBy(p => p.StockQuantity)
                            : queryable.OrderByDescending(p => p.StockQuantity);
                        break;
                    case "createddate":
                        queryable = isAscending
                            ? queryable.OrderBy(p => p.CreatedUtcDate)
                            : queryable.OrderByDescending(p => p.CreatedUtcDate);
                        break;
                    default:
                        // Default sort by created date descending
                        queryable = queryable.OrderByDescending(p => p.CreatedUtcDate);
                        break;
                }
            }
            else
            {
                // Default sort by created date descending
                queryable = queryable.OrderByDescending(p => p.CreatedUtcDate);
            }

            var query = queryable
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    Size = p.Size,
                    Color = p.Color,
                    StockQuantity = p.StockQuantity,
                    Unit = p.Unit,
                    CategoryId = p.CategoryId,
                    CategoryName = _context.Categories.Where(c => c.Id.Equals(p.CategoryId)).FirstOrDefault().Title,
                    CostPrice = p.CostPrice,
                    SellingPrice = p.SellingPrice,
                    ProfitMargin = p.ProfitMargin,
                    Status = p.Status,
                    CreatedBy = p.CreatedBy,
                    CreatedDate = p.CreatedUtcDate.ToString(),
                    ProductTags = _context.Tags.Where(t => p.ProductTags.Any(pt => pt.TagId.Equals(t.Id))).Select(t => t.Title).ToList(),
                    ProductPhotos = p.ProductPhotos
                        .Select(pp => new ProductPhotoDto
                        {
                            Id = pp.Id,
                            ProductId = pp.ProductId,
                            FileName = pp.FileName,
                            OriginalFileName = pp.OriginalFileName,
                            ContentType = pp.ContentType,
                            FileSize = pp.FileSize,
                            BlobUrl = pp.BlobUrl,
                            IsPrimary = pp.IsPrimary,
                            DisplayOrder = pp.DisplayOrder,
                            CreatedBy = pp.CreatedBy,
                            CreatedDate = pp.CreatedUtcDate.ToString() // Use default ToString() which EF can translate
                        })
                        .OrderBy(pp => pp.DisplayOrder)
                        .ThenBy(pp => pp.CreatedDate) // Order by date instead of formatted string
                        .ToList()
                });

            var pagination = await new Pagination<ProductDto>().CreateAsync(query, request.PageNumber, request.PageSize, cancellationToken);
            return Response.Success(pagination);
        }
    }
}
