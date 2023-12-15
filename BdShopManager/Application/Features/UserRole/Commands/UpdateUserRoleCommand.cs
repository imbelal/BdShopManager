using Common.RequestWrapper;

namespace Application.Features.UserRole.Commands
{
    public class UpdateUserRoleCommand : ICommand<Guid>
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
    }
}
