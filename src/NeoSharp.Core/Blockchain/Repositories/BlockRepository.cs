using System.Collections.Generic;
using System.Threading.Tasks;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Blockchain.Repositories
{
    public class BlockRepository : IBlockRepository
    {
        #region Private Fields
        private readonly IRepository _repository;
        private readonly ITransactionRepository _transactionModel;
        #endregion

        #region Constructor
        public BlockRepository(IRepository repository, ITransactionRepository transactionModel)
        {
            this._repository = repository;
            _transactionModel = transactionModel;
        }
        #endregion

        #region IBlocksModel implementation 
        /// <inheritdoc />
        public async Task<uint> GetTotalBlockHeight()
        {
            return await this._repository.GetTotalBlockHeight();
        }

        /// <inheritdoc />
        public async Task<uint> GetTotalBlockHeaderHeight()
        {
            return await this._repository.GetTotalBlockHeaderHeight();
        }

        /// <inheritdoc />
        public async Task<Block> GetBlock(uint height)
        {
            var hash = await this.GetBlockHash(height);

            return hash == null ? null : await GetBlock(hash);
        }

        /// <inheritdoc />
        public async Task<Block> GetBlock(UInt256 hash)
        {
            var header = await this._repository.GetBlockHeader(hash);

            if (header == null || header.Type != HeaderType.Extended) return null;

            var transactions = new Transaction[header.TransactionCount];

            for (int x = 0, m = header.TransactionCount; x < m; x++)
            {
                transactions[x] = await this._transactionModel.GetTransaction(header.TransactionHashes[x]);
            }

            header.Hash = hash;

            return header.GetBlock(transactions);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Block>> GetBlocks(IReadOnlyCollection<UInt256> blockHashes)
        {
            var blocks = new List<Block>();

            foreach (var hash in blockHashes)
            {
                var block = await this.GetBlock(hash);

                if (block == null) continue;

                blocks.Add(block);
            }

            return blocks;
        }

        /// <inheritdoc />
        public async Task<UInt256> GetBlockHash(uint height)
        {
            return await this._repository.GetBlockHashFromHeight(height);
        }

        /// <inheritdoc />
        public async Task<Block> GetNextBlock(UInt256 hash)
        {
            var header = await this._repository.GetBlockHeader(hash);

            if (header != null)
            {
                return await this.GetBlock(header.Index + 1);
            }

            return null;
        }

        /// <inheritdoc />
        public async Task<UInt256> GetNextBlockHash(UInt256 hash)
        {
            var header = await this._repository.GetBlockHeader(hash);

            if (header != null)
            {
                return await this._repository.GetBlockHashFromHeight(header.Index + 1);
            }

            return UInt256.Zero;
        }

        /// <inheritdoc />
        public async Task<long> GetSysFeeAmount(uint height)
        {
            return GetSysFeeAmount(await GetBlockHash(height));
        }

        /// <inheritdoc />
        public long GetSysFeeAmount(UInt256 hash)
        {
            return 0;
        }

        /// <inheritdoc />
        public async Task<BlockHeader> GetBlockHeader(uint height)
        {
            var hash = await this._repository.GetBlockHashFromHeight(height);

            if (hash != null) return await this.GetBlockHeader(hash);
            return null;
        }

        /// <inheritdoc />
        public async Task<BlockHeader> GetBlockHeader(UInt256 hash)
        {
            var header = await this._repository.GetBlockHeader(hash);

            if (header != null) header.Hash = hash;
            return header;
        }
        #endregion
    }
}