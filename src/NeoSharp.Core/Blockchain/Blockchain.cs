using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Caching;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.Core.TaskManagers;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Blockchain
{
    public class Blockchain : IDisposable, IBlockchain
    {
        #region Private fields

        private readonly IRepository _repository;
        private readonly IBinarySerializer _serializer;
        private readonly ICrypto _crypto;
        private CancellationTokenSource _cancelPersistTask;
        private int _initialized;

        #endregion

        #region Public fields

        public Block CurrentBlock { get; private set; }

        public BlockHeader LastBlockHeader { get; private set; }

        public static event EventHandler<Block> PersistCompleted;

        /// <inheritdoc />
        public StampedPool<UInt256, Transaction> MemoryPool { get; } =
            new StampedPool<UInt256, Transaction>(PoolMaxBehaviour.RemoveFromEnd, 50_000, tx => tx.Value.Hash, TransactionComparer);

        /// <inheritdoc />
        public Pool<uint, Block> BlockPool { get; } =
            new Pool<uint, Block>(PoolMaxBehaviour.RemoveFromEnd, 10_000, b => b.Index, (a, b) => a.Index.CompareTo(b.Index));

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repository">Repository</param>
        /// <param name="serializer">Serializer</param>
        /// <param name="crypto">Crypto</param>
        public Blockchain(IRepository repository, IBinarySerializer serializer, ICrypto crypto)
        {
            _repository = repository;
            _serializer = serializer;
            _crypto = crypto;
            _initialized = 0;
        }

        public async Task InitializeBlockchain()
        {
            if (Interlocked.Exchange(ref _initialized, 1) != 0)
            {
                return;
            }

            var bHeight = await _repository.GetTotalBlockHeight();
            var bHeader = await _repository.GetTotalBlockHeaderHeight();

            CurrentBlock = await GetBlock(bHeight);
            LastBlockHeader = await GetBlockHeader(bHeader);

            if (CurrentBlock == null || LastBlockHeader == null)
            {
                // This update last block header too

                await AddBlock(Genesis.GenesisBlock);
            }

            _cancelPersistTask = new CancellationTokenSource();

            // TODO: Check Task system

            IntervalScheduler.Run(TimeSpan.FromSeconds(1), _cancelPersistTask, async () =>
              {
                  while (await PersistBlock())
                  {

                  }
              });
        }

        static int TransactionComparer(Stamp<Transaction> a, Stamp<Transaction> b)
        {
            int c = 0;// TODO: by fee a.Value.NetworkFee.CompareTo(b.Value.NetworkFee);
            if (c == 0)
            {
                // TODO: Check ASC or DESC

                return a.Date.CompareTo(b.Date);
            }

            return c;
        }

        #region Blocks

        /// <inheritdoc />
        public Task<bool> AddBlock(Block block)
        {
            if (block.Hash == null)
            {
                block.UpdateHash(_serializer, _crypto);
            }

            if (CurrentBlock == null)
            {
                // Genesis

                BlockPool.Push(block);

                return PersistBlock();
            }

            // Small verification

            if (block.Timestamp <= CurrentBlock.Timestamp ||
                block.Index <= CurrentBlock.Index)
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(BlockPool.Push(block));
        }

        /// <inheritdoc />
        public async Task<bool> PersistBlock()
        {
            var block = BlockPool.PopFirstOrDefault();

            if (block == null) return false;

            if (block.Hash == null)
            {
                block.UpdateHash(_serializer, _crypto);
            }

            if (CurrentBlock != null)
            {
                // Do some checks

                if (block.Timestamp <= CurrentBlock.Timestamp ||
                    block.Index <= CurrentBlock.Index)
                {
                    return false;
                }

                if (block.PreviousBlockHash != CurrentBlock.Hash ||
                    block.Index - 1 != CurrentBlock.Index)
                {
                    // Send back to the pool

                    BlockPool.Push(block);

                    return false;
                }
            }

            var header = block.GetBlockHeader();

            foreach (var tx in block.Transactions)
            {
                await _repository.AddTransaction(tx);

                // Try to remove the TX from the pool

                MemoryPool.Remove(tx.Hash);
            }

            await _repository.AddBlockHeader(header);
            await _repository.SetTotalBlockHeight(block.Index);

            CurrentBlock = block;

            if (LastBlockHeader == null || LastBlockHeader.Index < block.Index)
            {
                LastBlockHeader = block;
                await _repository.SetTotalBlockHeaderHeight(block.Index);
            }

            PersistCompleted?.Invoke(this, block);

            return true;
        }

        /// <inheritdoc />
        public async Task<bool> ContainsBlock(UInt256 hash)
        {
            var header = await _repository.GetBlockHeader(hash);

            return header != null && header.Type == BlockHeader.HeaderType.Extended;
        }

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

            if (header == null || header.Type != BlockHeader.HeaderType.Extended) return null;

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
        public Task AddBlockHeaders(IEnumerable<BlockHeader> blockHeaders)
        {
            foreach (var header in blockHeaders)
            {
                // Update hash

                if (header.Hash == null)
                {
                    header.UpdateHash(_serializer, _crypto);
                }

                if (header.TransactionCount == 0)
                {
                    // We receive the header as extended on "BlockHeadersMessage" when is serialized from one complete Block
                    // but we want to know when is a header and when not, without checking the hashes

                    header.Type = BlockHeader.HeaderType.Header;
                }

                // Validate

                if (LastBlockHeader != null)
                {
                    if (LastBlockHeader.Index + 1 != header.Index ||
                        !LastBlockHeader.Hash.Equals(header.PreviousBlockHash))
                    {
                        break;
                    }
                }
                else
                {
                    if (LastBlockHeader.Index != 0 || !LastBlockHeader.Hash.Equals(Genesis.GenesisBlock.Hash))
                    {
                        break;
                    }
                }

                // Write

                _repository.AddBlockHeader(header);
                _repository.SetTotalBlockHeaderHeight(header.Index);

                LastBlockHeader = header;
            }

            return Task.CompletedTask;
        }

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

        // TODO [AboimPinto] Async methods cannot have out parameters. Method not used for now.
        ///// <inheritdoc />
        //public async Task<Transaction> GetTransaction(UInt256 hash, out int height)
        //{
        //    // TODO: How to get the height?

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

        public Task<bool> AddTransaction(Transaction transaction)
        {
            if (transaction.Hash == null)
            {
                transaction.UpdateHash(_serializer, _crypto);
            }

            // TODO: It is a bit more complicated

            MemoryPool.Push(transaction);

            return Task.FromResult(true);
        }

        /// <summary>
        /// Determine whether the specified transaction is included in the blockchain
        /// </summary>
        /// <param name="hash">Transaction hash</param>
        /// <returns>Return true if the specified transaction is included</returns>
        public async Task<bool> ContainsTransaction(UInt256 hash)
        {
            // TODO: Optimize this
            return await _repository.GetTransaction(hash) != null;
        }

        #endregion

        public static Fixed8 CalculateBonus(IEnumerable<CoinReference> inputs, bool ignoreClaimed = true)
        {
            return Fixed8.Zero;
        }

        public static Fixed8 CalculateBonus(IEnumerable<CoinReference> inputs, uint height_end)
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

        public MetaDataCache<T> GetMetaData<T>() where T : class, ISerializable, new()
        {
            return null;
        }

        /// <inheritdoc />
        public Contract GetContract(UInt160 hash)
        {
            return null;
        }

        /// <inheritdoc />
        public Asset GetAsset(UInt256 hash)
        {
            return null;
        }

        /// <inheritdoc />
        public IEnumerable<Asset> GetAssets()
        {
            yield break;
        }

        /// <inheritdoc />
        public IEnumerable<Contract> GetContracts()
        {
            yield break;
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

        private readonly List<ECPoint> _validators = new List<ECPoint>();

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
            return null;
        }

        public IEnumerable<TransactionOutput> GetUnspent(UInt256 hash)
        {
            return Enumerable.Empty<TransactionOutput>();
        }

        /// <summary>
        /// Determine if the transaction is double
        /// </summary>
        /// <param name="tx"></param>
        /// <returns></returns>
        public bool IsDoubleSpend(Transaction tx)
        {
            return false;
        }

        /// <summary>
        /// Called after the block was written to the repository
        /// </summary>
        /// <param name="block">区块</param>
        protected void OnPersistCompleted(Block block)
        {
            lock (_validators)
            {
                _validators.Clear();
            }

            PersistCompleted?.Invoke(this, block);
        }

        /// <summary>
        /// Clean resources
        /// </summary>
        public void Dispose()
        {
            _cancelPersistTask?.Cancel();
            _cancelPersistTask?.Dispose();
            _cancelPersistTask = null;
        }
    }
}