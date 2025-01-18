using Common.RequestWrapper;
using Domain.Dtos.Auth;

namespace Application.Features.User.Commands.Auth
{
    public class LoginUserCommand : ICommand<AuthenticateResponse>
    {
        public LoginUserRequest LoginUserRequest { get; set; }

        public LoginUserCommand(LoginUserRequest loginUserReq)
        {
            LoginUserRequest = loginUserReq;
        }
    }
}
