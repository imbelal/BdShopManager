using Application.Services.Auth.Dtos;
using Common.RequestWrapper;

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
