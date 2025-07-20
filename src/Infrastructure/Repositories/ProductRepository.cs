using Common.UnitOfWork;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Domain.Entities.Product>, IProductRepository
    {
        public ProductRepository(IApplicationDbContext applicationDbContext, IUnitOfWork unitOfWork) : base(applicationDbContext, unitOfWork)
        {
        }

        public override async Task<Domain.Entities.Product> GetByIdAsync(Guid Id, CancellationToken cancellationToken = default)
        {
            return await GetDbSet()
                .Include(p => p.ProductPhotos)
                .FirstOrDefaultAsync(p => p.Id == Id, cancellationToken);
        }

        public async Task<Domain.Entities.Product> GetByPhotoIdAsync(Guid photoId, CancellationToken cancellationToken = default)
        {
            return await GetDbSet()
            .Include(p => p.ProductPhotos)
                .FirstOrDefaultAsync(p => p.ProductPhotos.Any(pp => pp.Id == photoId), cancellationToken);
        }
    }
}
