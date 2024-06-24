using Common.UnitOfWork;
using Domain.Interfaces;

namespace Infrastructure.Repositories
{
    public class UserRoleRepository : GenericRepository<Domain.Entities.UserRole>, IUserRoleRepository
    {
        public UserRoleRepository(IApplicationDbContext applicationDbContext, IUnitOfWork unitOfWork) : base(applicationDbContext, unitOfWork)
        {
        }
    }
}
