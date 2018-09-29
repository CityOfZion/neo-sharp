using System.Linq;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Models.OperationManger
{
    public class BlockSigner : IBlockSigner
    {
        #region Private Fields
        private readonly Crypto _crypto;
        private readonly IBinarySerializer _binarySerializer;
        private readonly ITransactionOperationsManager _transactionOperationsManager;
        private readonly IWitnessOperationsManager _witnessOperationsManager;
        #endregion

        #region Constructor 
        public BlockSigner(
            Crypto crypto, 
            IBinarySerializer binarySerializer, 
            ITransactionOperationsManager transactionOperationsManager,
            IWitnessOperationsManager witnessOperationsManager)
        {
            this._crypto = crypto;
            this._binarySerializer = binarySerializer;
            this._transactionOperationsManager = transactionOperationsManager;
            this._witnessOperationsManager = witnessOperationsManager;
        }
        #endregion

        public void Sign(Block block)
        {
            // Compute tx hashes
            var txSize = block.Transactions?.Length ?? 0;
            block.TransactionHashes = new UInt256[txSize];

            for (var x = 0; x < txSize; x++)
            {
                this._transactionOperationsManager.Sign(block.Transactions?[x]);
                block.TransactionHashes[x] = block.Transactions?[x].Hash;
            }

            block.MerkleRoot = MerkleTree.ComputeRoot(block.TransactionHashes.ToArray());

            // Compute hash
            var serializedBlock = this._binarySerializer.Serialize(block, new BinarySerializerSettings
            {
                Filter = a => a != nameof(block.Witness) && 
                              a != nameof(block.Transactions) && 
                              a != nameof(block.TransactionHashes) && 
                              a != nameof(block.Type)
            });

            block.Hash = new UInt256(this._crypto.Hash256(serializedBlock));

            this._witnessOperationsManager.Sign(block.Witness);
        }
    }
}
