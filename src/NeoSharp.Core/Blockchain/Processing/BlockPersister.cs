using System;
using System.Threading.Tasks;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;

namespace NeoSharp.Core.Blockchain.Processing
{
    public class BlockPersister : IBlockPersister
    {
        #region Private Fields 
        private readonly IRepository _repository;
        private readonly IBlockHeaderPersister _blockHeaderPersister;
        private readonly ITransactionPersister<Transaction> _transactionPersister;
        private readonly ITransactionPool _transactionPool;
        private readonly ILogger<BlockPersister> _logger;
        #endregion

        #region Constructor 
        public BlockPersister(
            IRepository repository, 
            IBlockHeaderPersister blockHeaderPersister,
            ITransactionPersister<Transaction> transactionPersister,
            ITransactionPool transactionPool, 
            ILogger<BlockPersister> logger)
        {
            this._repository = repository;
            this._blockHeaderPersister = blockHeaderPersister;
            this._transactionPersister = transactionPersister;
            this._transactionPool = transactionPool;
            _logger = logger;
        }
        #endregion

        #region IBlockPersister Implementation 
        public Block LastPersistedBlock { get; private set; }

        public event EventHandler<BlockHeader[]> OnBlockHeadersPersisted;

        public async Task Persist(params Block[] blocks)
        {
            foreach (var block in blocks)
            {
                this.LastPersistedBlock = block;

                var blockHeader = await this._repository.GetBlockHeader(block.Hash);
                if (blockHeader == null)
                {
                    await this._blockHeaderPersister.Persist(block.GetBlockHeader());
                }

                foreach (var transaction in block.Transactions)
                {
                    await this._transactionPersister.Persist(transaction);
                    this._transactionPool.Remove(transaction.Hash);
                }
            }
        }

        public async Task Persist(params BlockHeader[] blockHeaders)
        {
            try
            {
                this._blockHeaderPersister.OnBlockHeadersPersisted += this.HandleBlockHandlePersisted;
                await this._blockHeaderPersister.Persist(blockHeaders);
            }
            finally
            {
                this._blockHeaderPersister.OnBlockHeadersPersisted -= this.HandleBlockHandlePersisted;
            }
        }

        public async Task<bool> IsBlockPersisted(Block block)
        {
            this._logger.LogDebug($"Verify if the {block.Hash} is already in the blockchain.");
            var blockHeader = await this._repository.GetBlockHeader(block.Hash);

            if (blockHeader?.Type == HeaderType.Extended)
            {
                throw new InvalidOperationException($"The block \"{block.Hash.ToString(true)}\" exists already on the blockchain.");
            }

            if (blockHeader != null && blockHeader.Hash != block.Hash)
            {
                throw new InvalidOperationException($"The block \"{block.Hash.ToString(true)}\" has an invalid hash.");
            }

            this._logger.LogDebug($"The block with the hash {block.Hash} is not int the blockchain.");
            return false;
        }
        #endregion

        #region Private Method 
        private void HandleBlockHandlePersisted(object sender, BlockHeader[] e)
        {
            this.OnBlockHeadersPersisted?.Invoke(sender, e);
        }
        #endregion
    }
}