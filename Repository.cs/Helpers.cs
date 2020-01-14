using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Repository
{
    public enum ProjectType
    {
        IdSrv = 1,
        Api = 2,
        Mvc = 3
    }

    public static class Helpers
    {
        private static string IdSrvIP = null;
        private static string MvcIP = null;
        private static string ApiIP = null;

        public static string GetIp(ProjectType p)
        {
            if (p == ProjectType.IdSrv)
                return IdSrvIP;
            else if (p == ProjectType.Api)
                return ApiIP;
            else
                return MvcIP;
        }

        public static void SetIp(ProjectType p)
        {
            if (p == ProjectType.IdSrv && string.IsNullOrEmpty(IdSrvIP))
                IdSrvIP = GetIpAddress();
            else if (p == ProjectType.Api && string.IsNullOrEmpty(ApiIP))
                ApiIP = GetIpAddress();
            else if (p == ProjectType.Mvc && string.IsNullOrEmpty(MvcIP))
                MvcIP = GetIpAddress();
        }

        private static string GetIpAddress()
        {
            string hostName = Dns.GetHostName();

            var addresses = Dns.GetHostAddresses(hostName).Where(c => c.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToList();

            return addresses[0].ToString();
        }

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
