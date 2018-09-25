using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NeoSharp.Core.Blockchain.Processing;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Models;
using NeoSharp.Core.Network;
using NeoSharp.Core.Persistence;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Blockchain
{
    public class Blockchain : IBlockchain, IDisposable
    {
        #region Private fields
        private readonly IRepository _repository;
        private readonly IBlockHeaderPersister _blockHeaderPersister;
        private readonly IBlockProcessor _blockProcessor;
        private readonly IBlockchainContext _blockchainContext;

        private int _initialized;
        private readonly List<ECPoint> _validators = new List<ECPoint>();
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repository">Repository</param>
        /// <param name="blockHeaderPersister">Block Header Persister</param>
        /// <param name="blockProcessor">Block Processor</param>
        /// <param name="blockchainContext">Block chain context class.</param>
        public Blockchain(
            IRepository repository,
            IBlockHeaderPersister blockHeaderPersister,
            IBlockProcessor blockProcessor,
            IBlockchainContext blockchainContext)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _blockHeaderPersister = blockHeaderPersister ?? throw new ArgumentNullException(nameof(blockHeaderPersister));
            _blockProcessor = blockProcessor ?? throw new ArgumentNullException(nameof(blockProcessor));
            _blockchainContext = blockchainContext ?? throw new ArgumentNullException(nameof(blockchainContext)); ;

            _blockHeaderPersister.OnBlockHeadersPersisted += (_, blockHeaders) => this._blockchainContext.LastBlockHeader = blockHeaders.Last();
            _blockProcessor.OnBlockProcessed += (_, block) => this._blockchainContext.CurrentBlock = block;
        }

        public async Task InitializeBlockchain()
        {
            if (Interlocked.Exchange(ref _initialized, 1) != 0)
            {
                return;
            }

            var blockHeight = await _repository.GetTotalBlockHeight();
            var blockHeaderHeight = await _repository.GetTotalBlockHeaderHeight();

            this._blockchainContext.CurrentBlock = await GetBlock(blockHeight);
            this._blockchainContext.LastBlockHeader = await GetBlockHeader(blockHeaderHeight);

            this._blockHeaderPersister.LastBlockHeader = this._blockchainContext.LastBlockHeader;

            this._blockProcessor.Run(this._blockchainContext.CurrentBlock);
            if (this._blockchainContext.CurrentBlock == null || this._blockchainContext.LastBlockHeader == null)
            {
                await this._blockProcessor.AddBlock(Genesis.GenesisBlock);
            }
        }

        #region Blocks

        /// <inheritdoc />
        public async Task<Block> GetBlock(uint height)
        {
            var hash = await GetBlockHash(height);

            return hash == null ? null : await GetBlock(hash);
        }

        /// <inheritdoc />
        public async Task<Block> GetBlock(UInt256 hash)
        {
            var header = await _repository.GetBlockHeader(hash);

            if (header == null || header.Type != HeaderType.Extended) return null;

            var transactions = new Transaction[header.TransactionCount];

            for (int x = 0, m = header.TransactionCount; x < m; x++)
            {
                transactions[x] = await _repository.GetTransaction(header.TransactionHashes[x]);
            }

            header.Hash = hash;

            return header.GetBlock(transactions);
        }

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

        #endregion

        #region BlockHeaders

        /// <inheritdoc />
        public async Task<BlockHeader> GetBlockHeader(uint height)
        {
            var hash = await _repository.GetBlockHashFromHeight(height);

            if (hash != null) return await GetBlockHeader(hash);

            return null;
        }

        /// <inheritdoc />
        public async Task<BlockHeader> GetBlockHeader(UInt256 hash)
        {
            var header = await _repository.GetBlockHeader(hash);

            if (header != null) header.Hash = hash;

            return header;
        }

        #endregion

        #region Transactions

        /// <inheritdoc />
        public async Task<Transaction> GetTransaction(UInt256 hash)
        {
            return await _repository.GetTransaction(hash);
        }

        // TODO #390 [AboimPinto] Async methods cannot have out parameters. Method not used for now.
        ///// <inheritdoc />
        //public async Task<Transaction> GetTransaction(UInt256 hash, out int height)
        //{
        //    // TODO #390: How to get the height?

        //    height = 0;
        //    return await _repository.GetTransaction(hash);
        //}

        public async Task<IEnumerable<Transaction>> GetTransactions(IReadOnlyCollection<UInt256> transactionHashes)
        {
            var transactions = new List<Transaction>();

            foreach (var hash in transactionHashes)
            {
                var tx = await GetTransaction(hash);

                if (tx == null) continue;
                transactions.Add(tx);
            }

            return transactions;
        }

        /// <inheritdoc />
        public async Task<bool> ContainsTransaction(UInt256 hash)
        {
            // TODO #389: Optimize this
            return await _repository.GetTransaction(hash) != null;
        }

        #endregion

        public static Fixed8 CalculateBonus(IEnumerable<CoinReference> inputs, bool ignoreClaimed = true)
        {
            return Fixed8.Zero;
        }

        public static Fixed8 CalculateBonus(IEnumerable<CoinReference> inputs, uint heightEnd)
        {
            return Fixed8.Zero;
        }

        public bool ContainsUnspent(CoinReference input)
        {
            return ContainsUnspent(input.PrevHash, input.PrevIndex);
        }

        public bool ContainsUnspent(UInt256 hash, ushort index)
        {
            return false;
        }

        /// <inheritdoc />
        public Task<Contract> GetContract(UInt160 hash)
        {
            return _repository.GetContract(hash);
        }

        /// <inheritdoc />
        public Task<Asset> GetAsset(UInt256 hash)
        {
            return _repository.GetAsset(hash);
        }

        /// <inheritdoc />
        public Task<IEnumerable<Asset>> GetAssets()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<IEnumerable<Contract>> GetContracts()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the contractor's contract address
        /// </summary>
        /// <param name="validators"></param>
        /// <returns></returns>
        public static UInt160 GetConsensusAddress(ECPoint[] validators)
        {
            return UInt160.Zero;
        }

        public ECPoint[] GetValidators()
        {
            lock (_validators)
            {
                if (_validators.Count == 0)
                {
                    _validators.AddRange(GetValidators(Enumerable.Empty<Transaction>()));
                }
                return _validators.ToArray();
            }
        }

        public virtual IEnumerable<ECPoint> GetValidators(IEnumerable<Transaction> others)
        {
            yield break;
        }

        /// <inheritdoc />
        public virtual async Task<long> GetSysFeeAmount(uint height)
        {
            return GetSysFeeAmount(await GetBlockHash(height));
        }

        /// <inheritdoc />
        public long GetSysFeeAmount(UInt256 hash)
        {
            return 0;
        }

        /// <inheritdoc />
        public TransactionOutput GetUnspent(UInt256 hash, ushort index)
        {
            var states = _repository.GetCoinStates(hash).Result;

            if (states == null || index >= states.Length || states[index].HasFlag(CoinState.Spent))
            {
                return null;
            }

            return GetTransaction(hash).Result.Outputs[index];
        }

        public IEnumerable<TransactionOutput> GetUnspent(UInt256 hash)
        {
            var outputs = new List<TransactionOutput>();
            
            var states = _repository.GetCoinStates(hash).Result;
            if (states != null)
            {
                var tx = GetTransaction(hash).Result;
                for (var i = 0; i < states.Length; i++)
                {
                    if (!states[i].HasFlag(CoinState.Spent))
                    {
                        outputs.Add(tx.Outputs[i]);
                    }
                }
            }
            return outputs;
        }

        /// <inheritdoc />
        public bool IsDoubleSpend(Transaction tx)
        {
            if (tx.Inputs.Length == 0)
            {
                return false;
            }

            foreach (var group in tx.Inputs.GroupBy(p => p.PrevHash))
            {
                var states = _repository.GetCoinStates(group.Key).Result;

                if (states == null || group.Any(p => p.PrevIndex >= states.Length || states[p.PrevIndex].HasFlag(CoinState.Spent)))
                {
                    return true;
                }
            }
            
            return false;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (_initialized == 1)
            {
            }
        }
    }
}
