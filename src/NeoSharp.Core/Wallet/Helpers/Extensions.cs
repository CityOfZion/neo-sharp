using System;
using NeoSharp.Cryptography;
using NeoSharp.Types;

namespace NeoSharp.Core.Wallet.Helpers
{
    public static class Extensions
    {
        /// <summary>
        /// Converts a ScriptHash into a Address representation 
        /// ('A' + Base58Encoding(<paramref name="scriptHash"/>)
        /// </summary>
        /// <param name="scriptHash">Script hash.</param>
        /// <returns>The address.</returns>
        public static string ToAddress(this UInt160 scriptHash)
        {
            var data = new byte[21];
            data[0] = 0x17;
            Buffer.BlockCopy(scriptHash.ToArray(), 0, data, 1, 20);
            return Crypto.Default.Base58CheckEncode(data);
        }

        /// <summary>
        /// Check if the byte array can be used as a private key
        ///  http://gobittest.appspot.com/PrivateKey
        /// </summary>
        /// <returns><c>true</c>, if the byte array is valid <c>false</c> otherwise.</returns>
        /// <param name="privateKey">Private key.</param>
        public static bool IsValidPrivateKey(this byte[] privateKey)
        {
            //if data.LengthIsCorrect AND data.FirstByteIsCorrect AND data.LastByteIsCorrect
            var isValid = true;

            if (privateKey.Length != 34)
            {
                isValid = false;
            }

            if (privateKey[0] != 0x80)
            {
                isValid = false;
            }

            if (privateKey[33] != 0x01)
            {
                isValid = false;
            }

            return isValid;
        }

        public static void ValidateAccount(this IWalletAccount account)
        {
            if (account.Contract == null)
            {
                throw new ArgumentException("Empty contract");
            }

            if (account.Contract.ScriptHash == null)
            {
                throw new ArgumentException("Invalid Script Hash");
            }
        }
    }
}