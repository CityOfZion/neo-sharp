using System;
using System.Linq;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Models
{
    /// <summary>
    /// Header and complete TX data
    /// </summary>
    [Serializable]
    public class Block : BlockHeader
    {
        #region Serializable data

        /// <summary>
        /// Transactions
        /// </summary>
        [BinaryProperty(100, MaxLength = 0x10000, Override = true)]
        public Transaction[] Transactions;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public Block() : base(HeaderType.Extended) { }

        /// <summary>
        /// Update hash
        /// </summary>
        public override void UpdateHash()
        {
            // Compute tx hashes

            var txSize = Transactions?.Length ?? 0;
            TransactionHashes = new UInt256[txSize];

            for (var x = 0; x < txSize; x++)
            {
                Transactions?[x].UpdateHash();
                TransactionHashes[x] = Transactions?[x].Hash;
            }

            MerkleRoot = MerkleTree.ComputeRoot(TransactionHashes.ToArray());

            // Compute hash

            var serializedBlock = BinarySerializer.Default.Serialize(this, new BinarySerializerSettings
            {
                Filter = a => a != nameof(Witness) && a != nameof(Transactions) && a != nameof(TransactionHashes) && a != nameof(Type)
            });

            Hash = new UInt256(Crypto.Default.Hash256(serializedBlock));

            Witness?.UpdateHash();
        }

        /// <summary>
        /// Get block header
        /// </summary>
        public BlockHeader GetBlockHeader()
        {
            return new BlockHeader(HeaderType.Extended)
            {
                ConsensusData = ConsensusData,
                Hash = Hash,
                Index = Index,
                MerkleRoot = MerkleRoot,
                NextConsensus = NextConsensus,
                PreviousBlockHash = PreviousBlockHash,
                Witness = Witness,
                Timestamp = Timestamp,
                TransactionHashes = Transactions?.Select(u => u.Hash).ToArray(),
                Version = Version
            };
        }
    }
}