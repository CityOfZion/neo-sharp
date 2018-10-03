using System.Linq;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Blockchain.Repositories;
using NeoSharp.Core.Cryptography;
using NeoSharp.Cryptography;
using NeoSharp.Types;

namespace NeoSharp.Core.Models.OperationManger
{
    public class BlockOperationManager : IBlockOperationsManager
    {
        #region Private Fields
        private readonly Crypto _crypto;
        private readonly IBinarySerializer _binarySerializer;
        private readonly ISigner<Transaction> _transactionSigner;
        private readonly IWitnessOperationsManager _witnessOperationsManager;
        private readonly IBlockRepository _blockRepository;

        #endregion

        #region Constructor 

        public BlockOperationManager
            (
            Crypto crypto,
            IBinarySerializer binarySerializer,
            ISigner<Transaction> transactionSigner,
            IWitnessOperationsManager witnessOperationsManager,
            IBlockRepository blockRepository
            )
        {
            _crypto = crypto;
            _binarySerializer = binarySerializer;
            _transactionSigner = transactionSigner;
            _witnessOperationsManager = witnessOperationsManager;
            _blockRepository = blockRepository;
        }

        #endregion

        #region IBlockOperationsManager implementation 

        public void Sign(Block block)
        {
            // Compute tx hashes
            var txSize = block.Transactions?.Length ?? 0;
            block.TransactionHashes = new UInt256[txSize];

            for (var x = 0; x < txSize; x++)
            {
                _transactionSigner.Sign(block.Transactions?[x]);
                block.TransactionHashes[x] = block.Transactions?[x].Hash;
            }

            block.MerkleRoot = MerkleTree.ComputeRoot(block.TransactionHashes.ToArray());

            // Compute hash
            var serializedBlock = _binarySerializer.Serialize(block, new BinarySerializerSettings
            {
                Filter = a => a != nameof(block.Witness) &&
                              a != nameof(block.Transactions) &&
                              a != nameof(block.TransactionHashes) &&
                              a != nameof(block.Type)
            });

            block.Hash = new UInt256(_crypto.Hash256(serializedBlock));

            _witnessOperationsManager.Sign(block.Witness);
        }

        public bool Verify(Block block)
        {
            var task = _blockRepository.GetBlockHeader(block.PreviousBlockHash);
            task.Wait();

            var prevHeader = task.Result;

            if (prevHeader == null)
            {
                return false;
            }

            if (prevHeader.Index + 1 != block.Index)
            {
                return false;
            }

            if (prevHeader.Timestamp >= block.Timestamp)
            {
                return false;
            }

            if (!_witnessOperationsManager.Verify(block.Witness))
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
