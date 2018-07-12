using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using NeoSharp.Core.Extensions;

namespace NeoSharp.Core.Cryptography
{
    public static class Helper
    {
        public static byte[] ToAesKey(ICrypto crypto, string password)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] passwordHash = crypto.Sha256(crypto.Sha256(passwordBytes));
            Array.Clear(passwordBytes, 0, passwordBytes.Length);
            return passwordHash;
        }

        public static byte[] ToAesKey(ICrypto crypto, SecureString password)
        {
            byte[] passwordBytes = password.ToByteArray();
            byte[] passwordHash = crypto.Sha256(crypto.Sha256(passwordBytes));
            Array.Clear(passwordBytes, 0, passwordBytes.Length);
            return passwordHash;
        }
    }
}