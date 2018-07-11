using System;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Types;
using Newtonsoft.Json;

namespace NeoSharp.Core.Models
{
    /// <summary>
    /// Header with TX hashes
    /// </summary>
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
        /// Constructor
        /// </summary>
        public BlockHeader() : base(HeaderType.Extended) { }

        /// <summary>
        /// Update hash
        /// </summary>
        /// <param name="serializer">Serializer</param>
        /// <param name="crypto">Crypto</param>
        public override void UpdateHash(IBinarySerializer serializer, ICrypto crypto)
        {
            if (MerkleRoot == null)
            {
                // Compute hash

                MerkleRoot = MerkleTree.ComputeRoot(crypto, TransactionHashes);
            }

            Hash = new UInt256(crypto.Hash256(serializer.Serialize(this, new BinarySerializerSettings()
            {
                Filter = (a) => a != nameof(Script) && a != nameof(Type) && a != nameof(TransactionHashes)
            })));

            Script?.UpdateHash(serializer, crypto);
        }

        /// <summary>
        /// Get block
        /// </summary>
        /// <param name="txs">Transactions</param>
        /// <returns>Return block</returns>
        public Block GetBlock(Transaction[] txs)
        {
            return new Block()
            {
                ConsensusData = ConsensusData,
                Index = Index,
                Hash = Hash,
                MerkleRoot = MerkleRoot,
                NextConsensus = NextConsensus,
                PreviousBlockHash = PreviousBlockHash,
                Script = Script,
                Timestamp = Timestamp,
                Version = Version,
                Transactions = txs,
            };
        }

        /// <summary>
        /// Get block header base
        /// </summary>
        /// <returns>Return block header base</returns>
        public BlockHeaderBase GetBlockHeaderBase()
        {
            return new BlockHeaderBase()
            {
                ConsensusData = ConsensusData,
                Index = Index,
                Hash = Hash,
                MerkleRoot = MerkleRoot,
                NextConsensus = NextConsensus,
                PreviousBlockHash = PreviousBlockHash,
                Script = Script,
                Timestamp = Timestamp,
                Version = Version
            };
        }
    }
}