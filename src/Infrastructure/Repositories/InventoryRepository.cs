using Common.UnitOfWork;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class InventoryRepository : GenericRepository<Domain.Entities.Inventory>, IInventoryRepository
    {
        private readonly IApplicationDbContext _applicationDbContext;
        public InventoryRepository(IApplicationDbContext applicationDbContext, IUnitOfWork unitOfWork) : base(applicationDbContext, unitOfWork)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<List<Inventory>> GetByProductIdsAsync(List<Guid> productIds, CancellationToken cancellationToken = default)
        {
            return await _applicationDbContext.Inventories.Where(i => productIds.Contains(i.ProductId)).ToListAsync(cancellationToken);
        }

    }
}
