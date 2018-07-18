using System;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Extensions;

namespace NeoSharp.Core.DI
{
    public class CryptoInitializer : ICryptoInitializer
    {
        public CryptoInitializer(ICrypto crypto)
        {
            ICrypto.Initialize(crypto);
        }
    }
}
