using System;
using System.Threading.Tasks;
using NeoSharp.Core.Blockchain.Repositories;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Models;

namespace NeoSharp.Core.Blockchain.Processing
{
    public class BlockPersister : IBlockPersister
    {
        #region Private Fields 

        private readonly IBlockRepository _blockRepository;
        private readonly IBlockHeaderPersister _blockHeaderPersister;
        private readonly ITransactionPersister<Transaction> _transactionPersister;
        private readonly ITransactionPool _transactionPool;
        private readonly ILogger<BlockPersister> _logger;

        #endregion

        #region Constructor 

        public BlockPersister(
            IBlockRepository blockRepository,
            IBlockHeaderPersister blockHeaderPersister,
            ITransactionPersister<Transaction> transactionPersister,
            ITransactionPool transactionPool,
            ILogger<BlockPersister> logger)
        {
            _blockRepository = blockRepository;
            _blockHeaderPersister = blockHeaderPersister;
            _transactionPersister = transactionPersister;
            _transactionPool = transactionPool;
            _logger = logger;
        }

        #endregion

        #region IBlockPersister Implementation 
        public Block LastPersistedBlock { get; private set; }

        public event EventHandler<BlockHeader[]> OnBlockHeadersPersisted;

        public async Task Persist(params Block[] blocks)
        {
            var index = await _blockRepository.GetTotalBlockHeight();

            foreach (var block in blocks)
            {
                var blockHeader = await _blockRepository.GetBlockHeader(block.Hash);

                if (
                    blockHeader == null ||
                    (blockHeader.Type == HeaderType.Header && blockHeader.Hash.Equals(block.Hash))
                    )
                {
                    LastPersistedBlock = block;
					if (block.GetBlockHeader().Type == HeaderType.Extended && block.Index > 0)
					{
						await _blockHeaderPersister.Update(block.GetBlockHeader());
					}
					else
					{
						await _blockHeaderPersister.Persist(block.GetBlockHeader());
					}
                    

                    if (index + 1 == block.Index)
                    {
                        await _blockRepository.SetTotalBlockHeight(block.Index);
                        index = block.Index;
                    }

                    foreach (var transaction in block.Transactions)
                    {
                        await _transactionPersister.Persist(transaction);
                        _transactionPool.Remove(transaction.Hash);
                    }
                }
            }
        }

        public async Task Persist(params BlockHeader[] blockHeaders)
        {
            try
            {
                _blockHeaderPersister.OnBlockHeadersPersisted += HandleBlockHandlePersisted;
                await _blockHeaderPersister.Persist(blockHeaders);
            }
            finally
            {
                _blockHeaderPersister.OnBlockHeadersPersisted -= HandleBlockHandlePersisted;
            }
        }

        public async Task<bool> IsBlockPersisted(Block block)
        {
            _logger.LogDebug($"Verify if the {block.Hash} is already in the blockchain.");
            var blockHeader = await _blockRepository.GetBlockHeader(block.Hash);

            if (blockHeader?.Type == HeaderType.Extended)
            {
                this._logger.LogDebug($"The block \"{block.Hash.ToString(true)}\" exists already on the blockchain.");
                return true;
            }

            if (blockHeader != null && blockHeader.Hash != block.Hash)
            {
                this._logger.LogDebug($"The block \"{block.Hash.ToString(true)}\" has an invalid hash.");       // <-- [AboimPinto] I'm not sure if this validation should be on this method.
                return true;
            }

            _logger.LogDebug($"The block with the hash {block.Hash} is not int the blockchain.");
            return false;
        }
        #endregion

        #region Private Method 
        private void HandleBlockHandlePersisted(object sender, BlockHeader[] e)
        {
            OnBlockHeadersPersisted?.Invoke(sender, e);
        }
        #endregion
    }
}