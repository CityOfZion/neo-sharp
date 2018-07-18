using System;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Extensions;

namespace NeoSharp.Core.DI
{
    public class CryptoInitializer : ICryptoInitializer
    {
        public CryptoInitializer(Crypto crypto)
        {
            Crypto.Initialize(crypto);
        }
    }
}
