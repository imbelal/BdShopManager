using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Dtos;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Product.Queries
{
    public class GetProductPhotosQueryHandler : IQueryHandler<GetProductPhotosQuery, List<ProductPhotoDto>>
    {
        private readonly IReadOnlyApplicationDbContext _context;

        public GetProductPhotosQueryHandler(IReadOnlyApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IResponse<List<ProductPhotoDto>>> Handle(GetProductPhotosQuery request, CancellationToken cancellationToken)
        {

            Domain.Entities.Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

            if (product == null)
                return Response.Fail<List<ProductPhotoDto>>("Product not found!!");

            var photos = await _context.ProductPhotos
                .Where(pp => pp.ProductId == request.ProductId)
                .OrderBy(pp => pp.DisplayOrder)
                .ThenBy(pp => pp.CreatedUtcDate)
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
                .ToListAsync(cancellationToken);

            return Response.Success(photos);
        }
    }
}