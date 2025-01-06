using Application.Services.Auth.Interfaces;
using Application.Services.Common;
using Domain.Entities;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Application.Services.Auth.Implementations
{
    public class AccessTokenService : IAccessTokenService
    {
        private readonly ITokenGenerator _tokenGenerator;
        private readonly AppSettings _appSettings;

        public AccessTokenService(ITokenGenerator tokenGenerator, IOptions<AppSettings> appSettings) =>
            (_tokenGenerator, _appSettings) = (tokenGenerator, appSettings.Value);

        public string Generate(User user)
        {
            List<Claim> claims = new()
            {
                new Claim("id", user.Id.ToString()),
                new Claim("email", user.Email),
                new Claim("username", user.Username),
                new Claim("role", user.UserRole.Title),
                new Claim("tenantId", user.TenantId.ToString()),
            };
            return _tokenGenerator.Generate(_appSettings.JwtSettings.Secret, _appSettings.JwtSettings.ValidIssuer, _appSettings.JwtSettings.ValidAudience,
                _appSettings.JwtSettings.TokenValidityInMinutes, claims);
        }
    }
}
