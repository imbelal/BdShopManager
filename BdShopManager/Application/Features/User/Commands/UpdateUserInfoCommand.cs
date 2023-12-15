using Common.RequestWrapper;

namespace Application.Features.User.Commands
{
    public class UpdateUserInfoCommand : ICommand<Guid>
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }
}
