using System;
using System.Linq;
using NeoSharp.BinarySerialization;

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