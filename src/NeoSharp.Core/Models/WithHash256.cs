using NeoSharp.BinarySerialization;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Types;
using Newtonsoft.Json;

namespace NeoSharp.Core.Models
{
    public abstract class WithHash256
    {
        private UInt256 _hash = null;

        // TODO: How to inject ICrypto and IBinarySerializer here ?

        static readonly ICrypto _crypto = new BouncyCastleCrypto();
        static readonly IBinarySerializer _serializer = new BinarySerializer();

        [JsonProperty("txid")]
        public UInt256 Hash
        {
            get
            {
                if (_hash == null)
                {
                    _hash = new UInt256(_crypto.Hash256(GetHashData(_serializer)));
                }
                return _hash;
            }
        }

        public abstract byte[] GetHashData(IBinarySerializer serializer);
    }
}