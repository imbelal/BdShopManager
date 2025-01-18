using Common.RequestWrapper;
using Domain.Enums;

namespace Application.Features.UserRole.Commands
{
    public class UpdateUserRoleCommand : ICommand<Guid>
    {
        public Guid Id { get; set; }
        public UserRoleType Type { get; set; }
    }
}
