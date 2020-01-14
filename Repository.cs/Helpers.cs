using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Repository
{
    public static class Helpers
    {
        private static byte[] GenerateSecurityStamp()
        {
            byte[] bytes = new byte[128 / 8];

            using (var keyGenerator = RandomNumberGenerator.Create())
            {
                keyGenerator.GetBytes(bytes);
                return bytes;
            }
        }

        public static PasswordResult EncryptPassword(string password)
        {
            var securityStamp = GenerateSecurityStamp();

            var securityStampString = Convert.ToBase64String(securityStamp);

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: securityStamp,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));

            return new PasswordResult { Hash = hashed, SecurityStamp = securityStampString };
        }

        public static PasswordResult EncryptPassword(string password, string securityStampString)
        {
            var securityStamp = Convert.FromBase64String(securityStampString);

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: securityStamp,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));

            return new PasswordResult { Hash = hashed, SecurityStamp = securityStampString };
        }
    }
}
