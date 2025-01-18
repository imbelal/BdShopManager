using Common.RequestWrapper;
using Domain.Enums;

namespace Application.Features.UserRole.Commands
{
    public class CreateUserRoleCommand : ICommand<Guid>
    {
        public UserRoleType Type { get; set; }
    }
}
