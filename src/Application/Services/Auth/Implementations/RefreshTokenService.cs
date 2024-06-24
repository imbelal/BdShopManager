using Application.Services.Auth.Interfaces;
using Application.Services.Common;
using Domain.Entities;
using Microsoft.Extensions.Options;

namespace Application.Services.Auth.Implementations
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly ITokenGenerator _tokenGenerator;
        private readonly AppSettings _appSettings;

        public RefreshTokenService(ITokenGenerator tokenGenerator, IOptions<AppSettings> appSettings) =>
            (_tokenGenerator, _appSettings) = (tokenGenerator, appSettings.Value);

        public string Generate(User user) => _tokenGenerator.Generate(_appSettings.JwtSettings.Secret,
            _appSettings.JwtSettings.ValidIssuer, _appSettings.JwtSettings.ValidAudience,
            _appSettings.JwtSettings.RefreshTokenValidityInDays);
    }
}
