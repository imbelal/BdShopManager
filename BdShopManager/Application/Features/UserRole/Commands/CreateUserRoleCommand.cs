using Common.RequestWrapper;

namespace Application.Features.UserRole.Commands
{
    public class CreateUserRoleCommand : ICommand<Guid>
    {
        public string Title { get; set; }
    }
}
