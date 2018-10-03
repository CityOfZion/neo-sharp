using System;
using System.Security;
using System.Text;
using NeoSharp.Core.Extensions;
using NeoSharp.Cryptography;

namespace NeoSharp.Core.Cryptography
{
    public static class Helper
    {
        /// <summary>
        /// Hash256 Password
        /// </summary>
        /// <param name="password">Password</param>
        /// <returns>byte array hash256</returns>
        public static byte[] ToAesKey(string password)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] passwordHash = Crypto.Default.Sha256(Crypto.Default.Sha256(passwordBytes));
            Array.Clear(passwordBytes, 0, passwordBytes.Length);
            return passwordHash;
        }

        /// <summary>
        /// Hash256 Password
        /// </summary>
        /// <param name="password">Password</param>
        /// <returns>byte array hash256</returns>
        public static byte[] ToAesKey(SecureString password)
        {
            byte[] passwordBytes = password.ToByteArray();
            byte[] passwordHash = Crypto.Default.Sha256(Crypto.Default.Sha256(passwordBytes));
            Array.Clear(passwordBytes, 0, passwordBytes.Length);
            return passwordHash;
        }
    }
}