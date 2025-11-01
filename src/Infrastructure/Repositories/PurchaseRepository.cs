using Common.UnitOfWork;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PurchaseRepository : GenericRepository<Purchase>, IPurchaseRepository
    {
        private readonly IApplicationDbContext _applicationDbContext;

        public PurchaseRepository(IApplicationDbContext applicationDbContext, IUnitOfWork unitOfWork) : base(applicationDbContext, unitOfWork)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async override Task<Purchase> GetByIdAsync(Guid Id, CancellationToken cancellationToken = default)
        {
            return await _applicationDbContext.Purchases.Include(purchase => purchase.PurchaseItems)
                                                      .Where(purchase => purchase.Id == Id)
                                                      .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
