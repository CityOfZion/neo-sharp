using System;
using System.Linq;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Models
{
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
        /// Update hash
        /// </summary>
        /// <param name="serializer">Serializer</param>
        /// <param name="crypto">Crypto</param>
        public override void UpdateHash(IBinarySerializer serializer, ICrypto crypto)
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
            if (value == null) return null;

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