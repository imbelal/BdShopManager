using Application.Services.Common;
using Domain.Interfaces.Auth;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace Application.Services.Auth.Implementations
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16; // 128 bit 
        private const int KeySize = 32; // 256 bit
        private readonly AppSettings _appSettings;

        public PasswordHasher(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public string CreateHash(string password)
        {
            using (var algorithm = new Rfc2898DeriveBytes(password, SaltSize, _appSettings.Iterations, HashAlgorithmName.SHA512))
            {
                var key = Convert.ToBase64String(algorithm.GetBytes(KeySize));
                var salt = Convert.ToBase64String(algorithm.Salt);

                return $"{_appSettings.Iterations}.{salt}.{key}";
            }
        }

        public (bool Verified, bool NeedsUpgrade) VerifyPassword(string hash, string password)
        {
            var partsOfHash = hash.Split('.', 3);
            if (partsOfHash.Length != 3)
            {
                throw new FormatException("Unexpected hash format. " + "Should be formatted as `{ iterations }.{ salt}.{ hash}`");
            }

            var iterations = Convert.ToInt32(partsOfHash[0]);
            var salt = Convert.FromBase64String(partsOfHash[1]);
            var key = Convert.FromBase64String(partsOfHash[2]);

            var needsUpgrade = iterations != iterations;

            using (var algorithm = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA512))
            {
                var keyToCheck = algorithm.GetBytes(KeySize);

                var verified = keyToCheck.SequenceEqual(key);

                return (verified, needsUpgrade);
            }
        }
    }
}
