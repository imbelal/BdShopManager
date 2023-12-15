using Common.RequestWrapper;

namespace Application.Features.User.Commands
{
    public class ResetPasswordCommand : ICommand<Guid>
    {
        public Guid UserId { get; set; }
    }
}
