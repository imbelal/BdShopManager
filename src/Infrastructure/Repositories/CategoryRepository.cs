using Common.UnitOfWork;
using Domain.Interfaces;

namespace Infrastructure.Repositories
{
    public class CategoryRepository : GenericRepository<Domain.Entities.Category>, ICategoryRepository
    {
        public CategoryRepository(IApplicationDbContext applicationDbContext, IUnitOfWork unitOfWork) : base(applicationDbContext, unitOfWork)
        {
        }
    }
}
