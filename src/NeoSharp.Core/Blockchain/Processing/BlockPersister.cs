using System.Collections.Generic;
using System.Threading.Tasks;
using NeoSharp.Core.Blockchain.Repositories;
using NeoSharp.Core.Logging;
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
        private readonly ITransactionPool _transactionPool;
        private readonly ILogger<BlockPersister> _logger;
        #endregion

        #region Constructor 
        public BlockPersister(
            IBlockRepository blockRepository,
            IBlockchainContext blockchainContext,
            IBlockHeaderPersister blockHeaderPersister,
            ITransactionPersister<Transaction> transactionPersister,
            ITransactionPool transactionPool,
            ILogger<BlockPersister> logger)
        {
            _blockRepository = blockRepository;
            _blockchainContext = blockchainContext;
            _blockHeaderPersister = blockHeaderPersister;
            _transactionPersister = transactionPersister;
            _transactionPool = transactionPool;
            _logger = logger;
        }

        #endregion

        #region IBlockPersister Implementation 
        public async Task Persist(params Block[] blocks)
        {
            var index = await _blockRepository.GetTotalBlockHeight();

            foreach (var block in blocks)
            {
                var blockHeader = await _blockRepository.GetBlockHeader(block.Hash);

                if (blockHeader == null ||
                    blockHeader.Type == HeaderType.Header && blockHeader.Hash.Equals(block.Hash))
                {
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

                    _blockchainContext.CurrentBlock = block;
                }
            }
        }

        public async Task<IEnumerable<BlockHeader>> Persist(params BlockHeader[] blockHeaders)
        {
            return await _blockHeaderPersister.Persist(blockHeaders);
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
    }
}