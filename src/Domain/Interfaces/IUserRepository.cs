using Common.Repositories.Interfaces;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IUserRepository : IRepository<Domain.Entities.User>
    {
        void RemoveRefreshToken(RefreshToken refreshToken);
    }
}
