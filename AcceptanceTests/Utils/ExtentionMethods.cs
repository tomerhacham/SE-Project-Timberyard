using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace AcceptanceTests.Utils
{
    public static class ExtentionMethods
    {
        public static string HashString(this string value)
        {
            var sha256 = new SHA256CryptoServiceProvider();
            var hash_bytes = sha256.ComputeHash(Encoding.ASCII.GetBytes(value));

            return Encoding.ASCII.GetString(hash_bytes);
        }
    }
}
