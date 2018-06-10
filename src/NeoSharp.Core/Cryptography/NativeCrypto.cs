using System;
using System.Security.Cryptography;

namespace NeoSharp.Core.Cryptography
{
    public class NativeCrypto : BouncyCastleCrypto
    {
        /// <summary>
        /// Sha256 digests
        /// </summary>
        /// <param name="message">Message bytearray</param>
        /// <param name="offset">Offset</param>
        /// <param name="count">Length</param>
        /// <returns>Hash bytearray</returns>
        public override byte[] Sha256(byte[] message, int offset, int count)
        {
            byte[] hash;
            using (var sha = SHA256.Create())
            {
                hash = sha.ComputeHash(message, offset, count);
            }
            return hash;
        }

        /// <summary>
        /// Encrypt using ECB
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="key">Key</param>
        /// <returns>Bytearray</returns>
        public override byte[] AesEncrypt(byte[] data, byte[] key)
        {
            using (Aes aes = Aes.Create())
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

        /// <summary>
        /// Decrypt using ECB
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="key">Key</param>
        /// <returns>Bytearray</returns>
        public override byte[] AesDecrypt(byte[] data, byte[] key)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.None;

                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    return decryptor.TransformFinalBlock(data, 0, data.Length);
                }
            }
        }

        /// <summary>
        /// Encrypt using CBC
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="key">Key</param>
        /// <param name="iv">IV</param>
        /// <returns>Bytearray</returns>
        public override byte[] AesEncrypt(byte[] data, byte[] key, byte[] iv)
        {
            if (data == null || key == null || iv == null) throw new ArgumentNullException();
            if (data.Length % 16 != 0 || key.Length != 32 || iv.Length != 16) throw new ArgumentException();

            using (Aes aes = Aes.Create())
            {
                aes.Padding = PaddingMode.None;

                using (ICryptoTransform encryptor = aes.CreateEncryptor(key, iv))
                {
                    return encryptor.TransformFinalBlock(data, 0, data.Length);
                }
            }
        }

        /// <summary>
        /// Decrypt using CBC
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="key">Key</param>
        /// <param name="iv">IV</param>
        /// <returns>Bytearray</returns>
        public override byte[] AesDecrypt(byte[] data, byte[] key, byte[] iv)
        {
            if (data == null || key == null || iv == null) throw new ArgumentNullException();
            if (data.Length % 16 != 0 || key.Length != 32 || iv.Length != 16) throw new ArgumentException();

            using (Aes aes = Aes.Create())
            {
                aes.Padding = PaddingMode.None;

                using (ICryptoTransform decryptor = aes.CreateDecryptor(key, iv))
                {
                    return decryptor.TransformFinalBlock(data, 0, data.Length);
                }
            }
        }
    }
}