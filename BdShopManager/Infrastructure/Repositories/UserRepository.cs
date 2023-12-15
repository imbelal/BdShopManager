using Common.UnitOfWork;
using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<Domain.Entities.User>, IUserRepository
    {
        private readonly IApplicationDbContext _context;
        public UserRepository(IApplicationDbContext applicationDbContext, IUnitOfWork unitOfWork) : base(applicationDbContext, unitOfWork)
        {
            _context = applicationDbContext;
        }

        public void RemoveRefreshToken(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Remove(refreshToken);
        }
    }
}
