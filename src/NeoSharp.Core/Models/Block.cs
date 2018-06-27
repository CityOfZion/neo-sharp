using System;
using System.Linq;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Models
{
    [Serializable]
    public class Block : BlockHeader
    {
        /// <summary>
        /// Transactions
        /// </summary>
        [BinaryProperty(100, MaxLength = 0x10000)]
        public Transaction[] Transactions;

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

            TransactionHashes = Transactions.Select(u => u.Hash).ToArray();
            MerkleRoot = MerkleTree.ComputeRoot(crypto, TransactionHashes);

            Hash = new UInt256(crypto.Hash256(serializer.Serialize(this, new BinarySerializerSettings()
            {
                Filter = (a) => a != nameof(Script) && a != nameof(ScriptPrefix) && a != nameof(Transactions)
            })));

            Script?.UpdateHash(serializer, crypto);
        }
    }
}