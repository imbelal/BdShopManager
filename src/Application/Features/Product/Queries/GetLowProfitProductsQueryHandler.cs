using Common.Pagination;
using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Dtos;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Product.Queries
{
    public class GetLowProfitProductsQueryHandler : IQueryHandler<GetLowProfitProductsQuery, Pagination<ProductDto>>
    {
        private readonly IReadOnlyApplicationDbContext _context;

        public GetLowProfitProductsQueryHandler(IReadOnlyApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IResponse<Pagination<ProductDto>>> Handle(GetLowProfitProductsQuery request, CancellationToken cancellationToken)
        {
            // Calculate profit margin inline for EF Core translation
            var query = _context.Products
                .Include(p => p.ProductTags)
                .Include(p => p.ProductPhotos.Where(pp => pp.IsPrimary))
                .Where(p => p.SellingPrice > 0
                    ? (((p.SellingPrice - p.CostPrice) / p.SellingPrice) * 100) <= request.MaxProfitMargin
                    : 0 <= request.MaxProfitMargin) // Filter by low profit margin
                .OrderBy(p => p.SellingPrice > 0
                    ? ((p.SellingPrice - p.CostPrice) / p.SellingPrice) * 100
                    : 0) // Order by lowest margin first
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
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
                        .Where(pp => pp.IsPrimary)
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
                            CreatedDate = pp.CreatedUtcDate.ToString()
                        })
                        .OrderBy(pp => pp.DisplayOrder)
                        .ThenBy(pp => pp.CreatedDate)
                        .ToList()
                });

            var pagination = await new Pagination<ProductDto>()
                .CreateAsync(query, request.PageNumber, request.PageSize, cancellationToken);

            return Response.Success(pagination);
        }
    }
}
