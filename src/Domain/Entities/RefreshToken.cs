using Common.Entities;

namespace Domain.Entities
{
    public class RefreshToken : AuditableEntityBase<Guid>
    {
        public RefreshToken() : base()
        {

        }
        /// <summary>
        /// Initializes new instance of <see cref="RefreshToken"/>.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="token">The refresh token.</param>
        public RefreshToken(Guid userId, string token) : base(Guid.NewGuid())
        {
            UserId = userId;
            Token = token;
        }

        /// <summary>
        /// Gets or sets user primary key.
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// Gets or sets refresh token.
        /// </summary>
        public string Token { get; set; }
    }
}
