using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using NewsHub.Application.Interfaces.Security;
using System.Security.Cryptography;

namespace NewsHub.Infrastructure.Security
{
    public class PasswordHasherService : IPasswordHasherService
    {
        private const int IterationCount = 100_000;
        private const int SaltSize = 128 / 8;
        private const int KeySize = 256 / 8;

        public string HashPassword(string password)
        {
            byte[] salt = new byte[SaltSize];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            byte[] hashBytes = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: IterationCount,
                numBytesRequested: KeySize);

            return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hashBytes)}";
        }

        public bool VerifyPassword(string hash, string password)
        {
            var parts = hash.Split('.');
            if (parts.Length != 2)
                return false;

            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] storedHashBytes = Convert.FromBase64String(parts[1]);

            byte[] hashToCheckBytes = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: IterationCount,
                numBytesRequested: KeySize);

            return CryptographicOperations.FixedTimeEquals(
                hashToCheckBytes,
                storedHashBytes
            );
        }
    }
}