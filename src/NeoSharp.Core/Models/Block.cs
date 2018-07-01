using System;
using System.Linq;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Types;
using Newtonsoft.Json;

namespace NeoSharp.Core.Models
{
    [Serializable]
    public class Block
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
        [BinaryProperty(6)]
        public byte ScriptPrefix;

        [BinaryProperty(7)]
        [JsonProperty("script")]
        public Witness Script;

        /// <summary>
        /// Transactions
        /// </summary>
        [BinaryProperty(100, MaxLength = 0x10000)]
        public Transaction[] Transactions;

        [JsonProperty("hash")]
        public UInt256 Hash { get; set; }

        #endregion

        /// <summary>
        /// Update hash
        /// </summary>
        /// <param name="serializer">Serializer</param>
        /// <param name="crypto">Crypto</param>
        public void UpdateHash(IBinarySerializer serializer, ICrypto crypto)
        {
            foreach (var tx in Transactions)
            {
                tx.UpdateHash(serializer, crypto);
            }

            var transactionHashes = Transactions.Select(u => u.Hash).ToArray();
            MerkleRoot = MerkleTree.ComputeRoot(crypto, transactionHashes);

            Hash = new UInt256(crypto.Hash256(serializer.Serialize(this, new BinarySerializerSettings()
            {
                Filter = (a) => a != nameof(Script) && a != nameof(ScriptPrefix) && a != nameof(Transactions)
            })));

            Script?.UpdateHash(serializer, crypto);
        }

        public static implicit operator BlockHeader(Block value)
        {
            return new BlockHeader()
            {
                ConsensusData = value.ConsensusData,
                Hash = value.Hash,
                Index = value.Index,
                MerkleRoot = value.MerkleRoot,
                NextConsensus = value.NextConsensus,
                PreviousBlockHash = value.PreviousBlockHash,
                Script = value.Script,
                ScriptPrefix = value.ScriptPrefix,
                Timestamp = value.Timestamp,
                TransactionHashes = value.Transactions?.Select(u => u.Hash).ToArray(),
                Version = value.Version
            };
        }
    }
}