using Common.RequestWrapper;

namespace Application.Features.UserRole.Commands
{
    public class DeleteUserRoleCommand : ICommand<Guid>
    {
        public Guid Id { get; set; }
    }
}
