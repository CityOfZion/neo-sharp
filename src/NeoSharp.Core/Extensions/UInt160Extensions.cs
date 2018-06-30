using System;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Extensions
{
    public static class UInt160Extensions
    {
        /// <summary>
        /// Transforms an scriptHash representation into an address
        /// </summary>
        /// <returns>The address.</returns>
        /// <param name="scriptHash">Script hash.</param>
        /// <param name="crypto">Crypto.</param>
        public static String ToAddress(this UInt160 scriptHash, ICrypto crypto)
        {
            //TODO Add documentation. 
            byte[] data = new byte[21];
            data[0] = byte.Parse("23");
            Buffer.BlockCopy(scriptHash.ToArray(), 0, data, 1, 20);
            return crypto.Base58CheckEncode(data);
        }
    }
}
