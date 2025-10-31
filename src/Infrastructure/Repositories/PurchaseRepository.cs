using Common.UnitOfWork;
using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.Repositories
{
    public class PurchaseRepository : GenericRepository<Purchase>, IPurchaseRepository
    {
        private readonly IApplicationDbContext _applicationDbContext;

        public PurchaseRepository(IApplicationDbContext applicationDbContext, IUnitOfWork unitOfWork) : base(applicationDbContext, unitOfWork)
        {
            _applicationDbContext = applicationDbContext;
        }
    }
}
