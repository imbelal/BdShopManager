using Common.RequestWrapper;
using Common.ResponseWrapper;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Product.Queries
{
    public class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, Domain.Entities.Product>
    {
        private readonly IReadOnlyApplicationDbContext _context;
        public GetProductByIdQueryHandler(IReadOnlyApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IResponse<Domain.Entities.Product>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .Include(p => p.ProductPhotos)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (product == null)
                return Response.Fail<Domain.Entities.Product>("No Product found!!");

            return Response.Success(product);
        }
    }
}
