using System.Collections.Generic;
using System.Threading.Tasks;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.Types;

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
            _repository = repository;
            _transactionModel = transactionModel;
        }
        #endregion

        #region IBlocksModel implementation 

        /// <inheritdoc />
        public async Task<uint> GetTotalBlockHeight()
        {
            return await _repository.GetTotalBlockHeight();
        }

        /// <inheritdoc />
        public async Task SetTotalBlockHeight(uint index)
        {
            await _repository.SetTotalBlockHeight(index);
        }

        /// <inheritdoc />
        public async Task<uint> GetTotalBlockHeaderHeight()
        {
            return await _repository.GetTotalBlockHeaderHeight();
        }

        /// <inheritdoc />
        public async Task<Block> GetBlock(uint height)
        {
            var hash = await GetBlockHash(height);

            return hash == UInt256.Zero ? null : await GetBlock(hash);
        }

        /// <inheritdoc />
        public async Task<Block> GetBlock(UInt256 hash)
        {
            var header = await _repository.GetBlockHeader(hash);

            if (header == null || header.Type != HeaderType.Extended) return null;

            var transactions = new Transaction[header.TransactionCount];

            for (int x = 0, m = header.TransactionCount; x < m; x++)
            {
                transactions[x] = await _transactionModel.GetTransaction(header.TransactionHashes[x]);
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
                var block = await GetBlock(hash);

                if (block == null) continue;

                blocks.Add(block);
            }

            return blocks;
        }

        /// <inheritdoc />
        public async Task<UInt256> GetBlockHash(uint height)
        {
            return await _repository.GetBlockHashFromHeight(height);
        }

        /// <inheritdoc />
        public async Task<Block> GetNextBlock(UInt256 hash)
        {
            var header = await _repository.GetBlockHeader(hash);

            if (header != null)
            {
                return await GetBlock(header.Index + 1);
            }

            return null;
        }

        /// <inheritdoc />
        public async Task<UInt256> GetNextBlockHash(UInt256 hash)
        {
            var header = await _repository.GetBlockHeader(hash);

            if (header != null)
            {
                return await _repository.GetBlockHashFromHeight(header.Index + 1);
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
            var hash = await _repository.GetBlockHashFromHeight(height);

            return hash == UInt256.Zero ? null : await GetBlockHeader(hash);
        }

        /// <inheritdoc />
        public async Task<BlockHeader> GetBlockHeader(UInt256 hash)
        {
            var header = await _repository.GetBlockHeader(hash);

            if (header != null) header.Hash = hash;
            return header;
        }
        #endregion
    }
}