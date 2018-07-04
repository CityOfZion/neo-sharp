using System;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Types;
using Newtonsoft.Json;

namespace NeoSharp.Core.Models
{
    [Serializable]
    public class BlockHeader : BlockHeaderBase
    {
        #region Serializable data

        [BinaryProperty(100)]
        [JsonProperty("txhashes")]
        public UInt256[] TransactionHashes { get; set; }

        #endregion

        #region Non serializable data

        [JsonProperty("txcount")]
        public int TransactionCount => TransactionHashes?.Length ?? 0;

        #endregion

        /// <summary>
        /// Update hash
        /// </summary>
        /// <param name="serializer">Serializer</param>
        /// <param name="crypto">Crypto</param>
        public override void UpdateHash(IBinarySerializer serializer, ICrypto crypto)
        {
            MerkleRoot = MerkleTree.ComputeRoot(crypto, TransactionHashes);

            Hash = new UInt256(crypto.Hash256(serializer.Serialize(this, new BinarySerializerSettings()
            {
                Filter = (a) => a != nameof(Script) && a != nameof(ScriptPrefix) && a != nameof(TransactionHashes)
            })));

            Script?.UpdateHash(serializer, crypto);
        }
    }
}