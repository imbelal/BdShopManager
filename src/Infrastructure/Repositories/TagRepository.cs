using Common.UnitOfWork;
using Domain.Interfaces;

namespace Infrastructure.Repositories
{
    public class TagRepository : GenericRepository<Domain.Entities.Tag>, ITagRepository
    {
        public TagRepository(IApplicationDbContext applicationDbContext, IUnitOfWork unitOfWork) : base(applicationDbContext, unitOfWork)
        {
        }
    }
}
