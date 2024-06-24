using System.Security.Claims;

namespace Common.Services.Interfaces
{
    public interface ICurrentUserService
    {
        ClaimsPrincipal GetUser();
    }
}
