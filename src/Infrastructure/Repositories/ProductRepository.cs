using Common.UnitOfWork;
using Domain.Interfaces;

namespace Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Domain.Entities.Product>, IProductRepository
    {
        public ProductRepository(IApplicationDbContext applicationDbContext, IUnitOfWork unitOfWork) : base(applicationDbContext, unitOfWork)
        {
        }
    }
}
