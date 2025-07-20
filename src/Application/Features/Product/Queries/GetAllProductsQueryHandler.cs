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
            var queryable = _context.Products
                .Include(p => p.ProductTags)
                .Include(p => p.ProductPhotos)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    Unit = p.Unit,
                    CategoryId = p.CategoryId,
                    CategoryName = _context.Categories.FirstOrDefault(c => c.Id.Equals(p.CategoryId)).Title,
                    CreatedBy = p.CreatedBy,
                    CreatedDate = p.CreatedUtcDate.ToString("F"),
                    ProductTags = _context.Tags.Where(t => p.ProductTags.Select(x => x.TagId).Contains(t.Id)).Select(x => x.Title).ToList(),
                    ProductPhotos = p.ProductPhotos.Select(pp => new ProductPhotoDto
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
                        CreatedDate = pp.CreatedUtcDate.ToString("F")
                    }).OrderBy(pp => pp.DisplayOrder).ThenBy(pp => pp.CreatedDate).ToList()
                });

            var pagination = await new Pagination<ProductDto>().CreateAsync(queryable, request.PageNumber, request.PageSize, cancellationToken);
            return Response.Success(pagination);
        }
    }
}
