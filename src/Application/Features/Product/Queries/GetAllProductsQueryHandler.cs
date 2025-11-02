using Common.Pagination;
using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Dtos;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

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
                .Include(p => p.ProductTags)
                .Include(p => p.ProductPhotos.Where(pp => pp.IsPrimary)) // Only load primary photos
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
                    CreatedBy = p.CreatedBy,
                    CreatedDate = p.CreatedUtcDate.ToString(),
                    ProductTags = _context.Tags.Where(t => p.ProductTags.Any(pt => pt.TagId.Equals(t.Id))).Select(t => t.Title).ToList(),
                    ProductPhotos = p.ProductPhotos
                        .Where(pp => pp.IsPrimary) // Only include primary photos
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
