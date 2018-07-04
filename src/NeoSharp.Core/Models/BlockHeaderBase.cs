using System;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Types;
using Newtonsoft.Json;

namespace NeoSharp.Core.Models
{
    [Serializable]
    public class BlockHeaderBase
    {
        #region Serializable data

        [BinaryProperty(0)]
        [JsonProperty("version")]
        public uint Version;

        [BinaryProperty(1)]
        [JsonProperty("previousblockhash")]
        public UInt256 PreviousBlockHash;

        [BinaryProperty(2)]
        [JsonProperty("merkleroot")]
        public UInt256 MerkleRoot;

        [BinaryProperty(3)]
        [JsonProperty("time")]
        public uint Timestamp;

        [BinaryProperty(4)]
        [JsonProperty("index")]
        public uint Index;

        [BinaryProperty(5)]
        [JsonProperty("nonce")]
        public ulong ConsensusData;

        [BinaryProperty(6)]
        [JsonProperty("nextconsensus")]
        public UInt160 NextConsensus;

        /// <summary>
        /// Required for NEO serialization, without sense
        /// </summary>
        [BinaryProperty(7)]
        public byte ScriptPrefix;

        [BinaryProperty(8)]
        [JsonProperty("script")]
        public Witness Script;

        #endregion

        #region Non serializable data

        [JsonProperty("hash")]
        public UInt256 Hash { get; set; }

        #endregion

        /// <summary>
        /// Update hash
        /// </summary>
        /// <param name="serializer">Serializer</param>
        /// <param name="crypto">Crypto</param>
        public virtual void UpdateHash(IBinarySerializer serializer, ICrypto crypto)
        {
            Hash = new UInt256(crypto.Hash256(serializer.Serialize(this, new BinarySerializerSettings()
            {
                Filter = (a) => a != nameof(Script) && a != nameof(ScriptPrefix)
            })));

            Script?.UpdateHash(serializer, crypto);
        }
    }
}