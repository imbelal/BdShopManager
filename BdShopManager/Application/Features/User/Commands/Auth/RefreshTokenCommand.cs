using Application.Services.Auth.Dtos;
using Common.RequestWrapper;

namespace Application.Features.User.Commands.Auth
{
    public class RefreshTokenCommand : ICommand<AuthenticateResponse>
    {
        public RefreshTokenRequest RefreshTokenRequest { get; set; }

        public RefreshTokenCommand(RefreshTokenRequest refreshTokenReq)
        {
            RefreshTokenRequest = refreshTokenReq;
        }
    }
}
