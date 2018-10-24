using System.Linq;
using System.Threading.Tasks;
using NeoSharp.Core.Blockchain.Repositories;
using NeoSharp.Core.Models;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Blockchain.Processing
{
    public class BlockPersister : IBlockPersister
    {
        #region Private Fields

        private readonly IBlockRepository _blockRepository;
        private readonly IBlockchainContext _blockchainContext;
        private readonly IBlockHeaderPersister _blockHeaderPersister;
        private readonly ITransactionPersister<Transaction> _transactionPersister;
        
        #endregion

        #region Constructor

        public BlockPersister(
            IBlockRepository blockRepository,
            IBlockchainContext blockchainContext,
            IBlockHeaderPersister blockHeaderPersister,
            ITransactionPersister<Transaction> transactionPersister)
        {
            _blockRepository = blockRepository;
            _blockchainContext = blockchainContext;
            _blockHeaderPersister = blockHeaderPersister;
            _transactionPersister = transactionPersister;
        }

        #endregion

        #region IBlockPersister Implementation 

        public async Task Persist(params Block[] blocks)
        {
            var height = await _blockRepository.GetTotalBlockHeight();

            foreach (var block in blocks)
            {
                var blockHeader = await _blockRepository.GetBlockHeader(block.Hash);
                if (blockHeader != null && blockHeader.Type == HeaderType.Extended) continue;

                foreach (var transaction in block.Transactions)
                {
                    await _transactionPersister.Persist(transaction);
                }

                if (block.Index > 0)
                {
                    await _blockHeaderPersister.Update(block.GetBlockHeader());
                }
                else
                {
                    await _blockHeaderPersister.Persist(block.GetBlockHeader());
                }

                if (height + 1 == block.Index)
                {
                    await _blockRepository.SetTotalBlockHeight(block.Index);
                    height = block.Index;
                }

                _blockchainContext.CurrentBlock = block;
            }
        }

        #endregion
    }
}