﻿using Domain.Interfaces.Auth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services.Auth.Implementations
{
    public class TokenGenerator : ITokenGenerator
    {
        public string Generate(string secretKey, string issuer, string audience, double expires, IEnumerable<Claim> claims = null)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken securityToken = new(issuer, audience,
                claims,
                DateTime.UtcNow,
                DateTime.UtcNow.AddMinutes(expires),
                credentials);
            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }
    }
}
