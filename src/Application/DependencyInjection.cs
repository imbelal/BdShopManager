using Application.Services.Auth.Implementations;
using Application.Services.Auth.Interfaces;
using Common.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application
{
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddTransient<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ITokenGenerator, TokenGenerator>();
            services.AddScoped<IAccessTokenService, AccessTokenService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<IRefreshTokenValidator, RefreshTokenValidator>();
            services.AddScoped<IAuthenticateService, AuthenticateService>();
            services.AddTransient<ICurrentUserService, CurrentUserService>();
        }
    }
}
