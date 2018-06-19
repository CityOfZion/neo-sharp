using NeoSharp.BinarySerialization;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Persistence;
using NeoSharp.Core.Types;
using Newtonsoft.Json;

namespace NeoSharp.Core.Models
{
    public abstract class WithHash256 : NeoEntityBase
    {
        private UInt256 _hash;

        // TODO: How to inject ICrypto and IBinarySerializer here ?

        private static readonly ICrypto Crypto = new BouncyCastleCrypto();
        private static readonly IBinarySerializer Serializer = new BinarySerializer();

        [JsonProperty("txid")]
        public override UInt256 Hash
        {
            get
            {
                if (_hash == null)
                {
                    _hash = new UInt256(Crypto.Hash256(GetHashData(Serializer)));
                }

                return _hash;
            }
            set
            {
                _hash = value;
            }
        }

        public abstract byte[] GetHashData(IBinarySerializer serializer);
    }
}