using Domain.Entities;

namespace Domain.Interfaces.Auth
{
    /// <summary>
    /// Interface for generating token.
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Generates token based on user information.
        /// </summary>
        /// <param name="user"><see cref="User"/> instance.</param>
        /// <returns>Generated token.</returns>
        string Generate(User user);
    }
    /// <inheritdoc cref="ITokenService"/>
    public interface IRefreshTokenService : ITokenService { }
    /// <inheritdoc cref="ITokenService"/>
    public interface IAccessTokenService : ITokenService { }
}
