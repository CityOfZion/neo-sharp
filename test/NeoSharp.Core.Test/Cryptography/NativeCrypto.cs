using System;
using System.Security.Cryptography;
using NeoSharp.Cryptography;

namespace NeoSharp.Core.Test.Cryptography
{
    public class NativeCrypto : BouncyCastleCrypto
    {
        /// <inheritdoc />
        public override byte[] Sha256(byte[] message, int offset, int count)
        {
            byte[] hash;
            using (var sha = SHA256.Create())
            {
                hash = sha.ComputeHash(message, offset, count);
            }
            return hash;
        }

        /// <inheritdoc />
        public override byte[] AesEncrypt(byte[] data, byte[] key)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.None;

                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                {
                    return encryptor.TransformFinalBlock(data, 0, data.Length);
                }
            }
        }

        /// <inheritdoc />
        public override byte[] AesDecrypt(byte[] data, byte[] key)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.None;

                using (var decryptor = aes.CreateDecryptor())
                {
                    return decryptor.TransformFinalBlock(data, 0, data.Length);
                }
            }
        }

        /// <inheritdoc />
        public override byte[] AesEncrypt(byte[] data, byte[] key, byte[] iv)
        {
            if (data == null || key == null || iv == null) throw new ArgumentNullException();
            if (data.Length % 16 != 0 || key.Length != 32 || iv.Length != 16) throw new ArgumentException();

            using (var aes = Aes.Create())
            {
                aes.Padding = PaddingMode.None;

                using (var encryptor = aes.CreateEncryptor(key, iv))
                {
                    return encryptor.TransformFinalBlock(data, 0, data.Length);
                }
            }
        }

        /// <inheritdoc />
        public override byte[] AesDecrypt(byte[] data, byte[] key, byte[] iv)
        {
            if (data == null || key == null || iv == null) throw new ArgumentNullException();
            if (data.Length % 16 != 0 || key.Length != 32 || iv.Length != 16) throw new ArgumentException();

            using (var aes = Aes.Create())
            {
                aes.Padding = PaddingMode.None;

                using (var decryptor = aes.CreateDecryptor(key, iv))
                {
                    return decryptor.TransformFinalBlock(data, 0, data.Length);
                }
            }
        }
    }
}