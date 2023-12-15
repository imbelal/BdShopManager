using Application.Services.Auth.Interfaces;
using Application.Services.Common;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Application.Services.Auth.Implementations
{
    public class RefreshTokenValidator : IRefreshTokenValidator
    {
        private readonly AppSettings _appSettings;

        public RefreshTokenValidator(IOptions<AppSettings> appSettings) => _appSettings = appSettings.Value;

        public bool Validate(string refreshToken)
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JwtSettings.Secret)),
                ValidIssuer = _appSettings.JwtSettings.ValidIssuer,
                ValidAudience = _appSettings.JwtSettings.ValidAudience,
                ClockSkew = TimeSpan.Zero
            };

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new();
            try
            {
                jwtSecurityTokenHandler.ValidateToken(refreshToken, validationParameters,
                    out SecurityToken validatedToken);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
