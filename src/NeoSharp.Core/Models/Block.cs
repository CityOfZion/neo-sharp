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
    public class Block : BlockHeaderBase
    {
        #region Serializable data

        /// <summary>
        /// Transactions
        /// </summary>
        [BinaryProperty(100, MaxLength = 0x10000)]
        public Transaction[] Transactions;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public Block() : base(HeaderType.Extended) { }

        /// <summary>
        /// Update hash
        /// </summary>
        /// <param name="serializer">Serializer</param>
        /// <param name="crypto">Crypto</param>
        public override void UpdateHash(IBinarySerializer serializer, ICrypto crypto)
        {
            // Compute tx hashes

            foreach (var tx in Transactions)
            {
                tx.UpdateHash(serializer, crypto);
            }

            MerkleRoot = MerkleTree.ComputeRoot(crypto, Transactions.Select(u => u.Hash).ToArray());

            // Compute hash

            Hash = new UInt256(crypto.Hash256(serializer.Serialize(this, new BinarySerializerSettings()
            {
                Filter = (a) => a != nameof(Script) && a != nameof(Transactions) && a != nameof(Type)
            })));

            Script?.UpdateHash(serializer, crypto);
        }

        /// <summary>
        /// Get block header
        /// </summary>
        public BlockHeader GetBlockHeader()
        {
            return new BlockHeader()
            {
                ConsensusData = ConsensusData,
                Hash = Hash,
                Index = Index,
                MerkleRoot = MerkleRoot,
                NextConsensus = NextConsensus,
                PreviousBlockHash = PreviousBlockHash,
                Script = Script,
                Timestamp = Timestamp,
                TransactionHashes = Transactions?.Select(u => u.Hash).ToArray(),
                Version = Version
            };
        }

        /// <summary>
        /// Get block header base
        /// </summary>
        public BlockHeaderBase GetBlockHeaderBase()
        {
            return new BlockHeaderBase()
            {
                ConsensusData = ConsensusData,
                Hash = Hash,
                Index = Index,
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