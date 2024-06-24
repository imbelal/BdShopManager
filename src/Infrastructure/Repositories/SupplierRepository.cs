using Common.UnitOfWork;
using Domain.Interfaces;

namespace Infrastructure.Repositories
{
    public class SupplierRepository : GenericRepository<Domain.Entities.Supplier>, ISupplierRepository
    {
        public SupplierRepository(IApplicationDbContext applicationDbContext, IUnitOfWork unitOfWork) : base(applicationDbContext, unitOfWork)
        {
        }
    }
}
