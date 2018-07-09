using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using NeoSharp.Core.Caching;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Blockchain
{
    public class Blockchain : IDisposable, IBlockchain
    {
        private readonly IRepository _repository;

        public static event EventHandler<Block> PersistCompleted;

        /// <inheritdoc />
        public StampedPool<UInt256, Transaction> MemoryPool { get; } =
            new StampedPool<UInt256, Transaction>(PoolMaxBehaviour.RemoveFromEnd, 50_000, tx => tx.Value.Hash, TransactionComparer);

        /// <summary>
        /// The interval at which each block is generated, in seconds
        /// </summary>
        public const uint SecondsPerBlock = 15;

        /// <summary>
        /// Generate interval for each block
        /// </summary>
        public static readonly TimeSpan TimePerBlock = TimeSpan.FromSeconds(SecondsPerBlock);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repository">Repository</param>
        public Blockchain(IRepository repository)
        {
            this._repository = repository;
        }

        public async Task InitializeBlockchain()
        {
            LastBlockHeader = CurrentBlock = await GetBlock(0);

            if (CurrentBlock == null)
            {
                AddBlock(Genesis.GenesisBlock);
            }
        }

        public Block CurrentBlock { get; private set; }

        public BlockHeaderBase LastBlockHeader { get; private set; }

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
        public bool AddBlock(Block block)
        {
            if (CurrentBlock != null)
            {
                // Do some checks

                if (block.PreviousBlockHash != CurrentBlock.Hash ||
                    block.Timestamp < CurrentBlock.Timestamp ||
                    block.Index - 1 != CurrentBlock.Index)
                {
                    return false;
                }
            }

            LastBlockHeader = CurrentBlock = block;

            _repository.AddBlockHeader(LastBlockHeader);
            _repository.SetTotalBlockHeight(block.Index);

            foreach (var tx in block.Transactions)
            {
                _repository.AddTransaction(tx);
            }

            return true;
        }

        /// <inheritdoc />
        public bool ContainsBlock(UInt256 hash)
        {
            return _repository.GetBlockHeader(hash) != null;
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

            if (header != null)
            {
                var transactions = new List<Transaction>();
                foreach (var transactionHash in header.TransactionHashes)
                {
                    var transaction = await this._repository.GetTransaction(transactionHash);
                    transactions.Add(transaction);
                }

                return new Block
                {
                    ConsensusData = header.ConsensusData,
                    Index = header.Index,
                    Hash = header.Hash,
                    MerkleRoot = header.MerkleRoot,
                    NextConsensus = header.NextConsensus,
                    PreviousBlockHash = header.PreviousBlockHash,
                    Script = header.Script,
                    ScriptPrefix = header.ScriptPrefix,
                    Timestamp = header.Timestamp,
                    Version = header.Version,
                    Transactions = transactions.ToArray(),
                };
            }

            return null;
        }

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
            return await _repository.GetBlockHashFromHeight(height);
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

        #endregion

        #region BlockHeaders

        /// <inheritdoc />
        public void AddBlockHeaders(IEnumerable<BlockHeaderBase> blockHeaders)
        {
            // TODO: finish this logic

            foreach (var header in blockHeaders)
            {
                _repository.AddBlockHeader(header);
            }
        }

        /// <inheritdoc />
        public async Task<BlockHeader> GetBlockHeader(uint height)
        {
            var hash = await this._repository.GetBlockHashFromHeight(height);

            if (hash != null) return await this._repository.GetBlockHeader(hash);

            return null;
        }

        /// <inheritdoc />
        public async Task<BlockHeader> GetBlockHeader(UInt256 hash)
        {
            return await _repository.GetBlockHeader(hash);
        }

        #endregion

        #region Transactions

        /// <inheritdoc />
        public async Task<Transaction> GetTransaction(UInt256 hash)
        {
            return await this._repository.GetTransaction(hash);
        }

        // TODO [AboimPinto] Async methods cannot have out parameters. Method not used for now.
        ///// <inheritdoc />
        //public async Task<Transaction> GetTransaction(UInt256 hash, out int height)
        //{
        //    // TODO: How to get the height?

        //    height = 0;
        //    return await this._repository.GetTransaction(hash);
        //}

        public async Task<IEnumerable<Transaction>> GetTransactions(IReadOnlyCollection<UInt256> transactionHashes)
        {
            var transactions = new List<Transaction>();

            foreach (var hash in transactionHashes)
            {
                var tx = await this.GetTransaction(hash);

                if (tx == null) continue;
                transactions.Add(tx);
            }

            return transactions;
        }

        /// <summary>
        /// Determine whether the specified transaction is included in the blockchain
        /// </summary>
        /// <param name="hash">Transaction hash</param>
        /// <returns>Return true if the specified transaction is included</returns>
        public bool ContainsTransaction(UInt256 hash)
        {
            // TODO: Optimize this

            return _repository.GetTransaction(hash) != null;
        }

        #endregion

        public static Fixed8 CalculateBonus(IEnumerable<CoinReference> inputs, bool ignoreClaimed = true)
        {
            return Fixed8.Zero;
            //List<SpentCoin> unclaimed = new List<SpentCoin>();
            //foreach (var group in inputs.GroupBy(p => p.PrevHash))
            //{
            //    Dictionary<ushort, SpentCoin> claimable = Default.GetUnclaimed(group.Key);
            //    if (claimable == null || claimable.Count == 0)
            //        if (ignoreClaimed)
            //            continue;
            //        else
            //            throw new ArgumentException();
            //    foreach (CoinReference claim in group)
            //    {
            //        if (!claimable.TryGetValue(claim.PrevIndex, out SpentCoin claimed))
            //            if (ignoreClaimed)
            //                continue;
            //            else
            //                throw new ArgumentException();
            //        unclaimed.Add(claimed);
            //    }
            //}
            //return CalculateBonusInternal(unclaimed);
        }

        public static Fixed8 CalculateBonus(IEnumerable<CoinReference> inputs, uint height_end)
        {
            return Fixed8.Zero;
            //List<SpentCoin> unclaimed = new List<SpentCoin>();
            //foreach (var group in inputs.GroupBy(p => p.PrevHash))
            //{
            //    Transaction tx = Default.GetTransactionByHash(group.Key, out int height_start);
            //    if (tx == null) throw new ArgumentException();
            //    if (height_start == height_end) continue;
            //    foreach (CoinReference claim in group)
            //    {
            //        if (claim.PrevIndex >= tx.Outputs.Length || !tx.Outputs[claim.PrevIndex].AssetId.Equals(GoverningToken.Hash))
            //            throw new ArgumentException();
            //        unclaimed.Add(new SpentCoin
            //        {
            //            Output = tx.Outputs[claim.PrevIndex],
            //            StartHeight = (uint)height_start,
            //            EndHeight = height_end
            //        });
            //    }
            //}
            //return CalculateBonusInternal(unclaimed);
        }

        //private static Fixed8 CalculateBonusInternal(IEnumerable<SpentCoin> unclaimed)
        //{
        //    Fixed8 amount_claimed = Fixed8.Zero;
        //    foreach (var group in unclaimed.GroupBy(p => new { p.StartHeight, p.EndHeight }))
        //    {
        //        uint amount = 0;
        //        uint ustart = group.Key.StartHeight / DecrementInterval;
        //        if (ustart < GenerationAmount.Length)
        //        {
        //            uint istart = group.Key.StartHeight % DecrementInterval;
        //            uint uend = group.Key.EndHeight / DecrementInterval;
        //            uint iend = group.Key.EndHeight % DecrementInterval;
        //            if (uend >= GenerationAmount.Length)
        //            {
        //                uend = (uint)GenerationAmount.Length;
        //                iend = 0;
        //            }
        //            if (iend == 0)
        //            {
        //                uend--;
        //                iend = DecrementInterval;
        //            }
        //            while (ustart < uend)
        //            {
        //                amount += (DecrementInterval - istart) * GenerationAmount[ustart];
        //                ustart++;
        //                istart = 0;
        //            }
        //            amount += (iend - istart) * GenerationAmount[ustart];
        //        }
        //        amount += (uint)(Default.GetSysFeeAmount(group.Key.EndHeight - 1) - (group.Key.StartHeight == 0 ? 0 : Default.GetSysFeeAmount(group.Key.StartHeight - 1)));
        //        amount_claimed += group.Sum(p => p.Value) / 100000000 * amount;
        //    }
        //    return amount_claimed;
        //}

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
        public Contract GetContract(UInt256 hash)
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
            // return Contract.CreateMultiSigRedeemScript(validators.Length - (validators.Length - 1) / 3, validators).ToScriptHash();
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
            return Enumerable.Empty<ECPoint>();
            //DataCache<UInt160, AccountState> accounts = GetStates<UInt160, AccountState>();
            //DataCache<ECPoint, ValidatorState> validators = GetStates<ECPoint, ValidatorState>();
            //MetaDataCache<ValidatorsCountState> validators_count = GetMetaData<ValidatorsCountState>();
            //foreach (Transaction tx in others)
            //{
            //    foreach (TransactionOutput output in tx.Outputs)
            //    {
            //        AccountState account = accounts.GetAndChange(output.ScriptHash, () => new AccountState(output.ScriptHash));
            //        if (account.Balances.ContainsKey(output.AssetId))
            //            account.Balances[output.AssetId] += output.Value;
            //        else
            //            account.Balances[output.AssetId] = output.Value;
            //        if (output.AssetId.Equals(GoverningToken.Hash) && account.Votes.Length > 0)
            //        {
            //            foreach (ECPoint pubkey in account.Votes)
            //                validators.GetAndChange(pubkey, () => new ValidatorState(pubkey)).Votes += output.Value;
            //            validators_count.GetAndChange().Votes[account.Votes.Length - 1] += output.Value;
            //        }
            //    }
            //    foreach (var group in tx.Inputs.GroupBy(p => p.PrevHash))
            //    {
            //        Transaction tx_prev = GetTransactionByHash(group.Key, out int height);
            //        foreach (CoinReference input in group)
            //        {
            //            TransactionOutput out_prev = tx_prev.Outputs[input.PrevIndex];
            //            AccountState account = accounts.GetAndChange(out_prev.ScriptHash);
            //            if (out_prev.AssetId.Equals(GoverningToken.Hash))
            //            {
            //                if (account.Votes.Length > 0)
            //                {
            //                    foreach (ECPoint pubkey in account.Votes)
            //                    {
            //                        ValidatorState validator = validators.GetAndChange(pubkey);
            //                        validator.Votes -= out_prev.Value;
            //                        if (!validator.Registered && validator.Votes.Equals(Fixed8.Zero))
            //                            validators.Delete(pubkey);
            //                    }
            //                    validators_count.GetAndChange().Votes[account.Votes.Length - 1] -= out_prev.Value;
            //                }
            //            }
            //            account.Balances[out_prev.AssetId] -= out_prev.Value;
            //        }
            //    }
            //    switch (tx)
            //    {
            //        case StateTransaction tx_state:
            //            foreach (StateDescriptor descriptor in tx_state.Descriptors)
            //                switch (descriptor.Type)
            //                {
            //                    case StateType.Account:
            //                        ProcessAccountStateDescriptor(descriptor, accounts, validators, validators_count);
            //                        break;
            //                    case StateType.Validator:
            //                        ProcessValidatorStateDescriptor(descriptor, validators);
            //                        break;
            //                }
            //            break;
            //    }
            //}

            //int count = (int)validators_count.Get().Votes.Select((p, i) => new
            //{
            //    Count = i,
            //    Votes = p
            //}).Where(p => p.Votes > Fixed8.Zero).ToArray().WeightedFilter(0.25, 0.75, p => p.Votes.GetData(), (p, w) => new
            //{
            //    p.Count,
            //    Weight = w
            //}).WeightedAverage(p => p.Count, p => p.Weight);
            //count = Math.Max(count, StandbyValidators.Length);
            //HashSet<ECPoint> sv = new HashSet<ECPoint>(StandbyValidators);
            //ECPoint[] pubkeys = validators.Find().Select(p => p.Value).Where(p => (p.Registered && p.Votes > Fixed8.Zero) || sv.Contains(p.PublicKey)).OrderByDescending(p => p.Votes).ThenBy(p => p.PublicKey).Select(p => p.PublicKey).Take(count).ToArray();
            //IEnumerable<ECPoint> result;
            //if (pubkeys.Length == count)
            //{
            //    result = pubkeys;
            //}
            //else
            //{
            //    HashSet<ECPoint> hashSet = new HashSet<ECPoint>(pubkeys);
            //    for (int i = 0; i < StandbyValidators.Length && hashSet.Count < count; i++)
            //        hashSet.Add(StandbyValidators[i]);
            //    result = hashSet;
            //}
            //return result.OrderBy(p => p);
        }

        //byte[] IScriptTable.GetScript(byte[] script_hash)
        //{
        //    return GetContract(new UInt160(script_hash)).Script;
        //}

        //public StorageItem GetStorageItem(StorageKey key);

        /// <inheritdoc />
        public virtual async Task<long> GetSysFeeAmount(uint height)
        {
            return this.GetSysFeeAmount(await GetBlockHash(height));
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

        //protected void ProcessAccountStateDescriptor(StateDescriptor descriptor, DataCache<UInt160, AccountState> accounts, DataCache<ECPoint, ValidatorState> validators, MetaDataCache<ValidatorsCountState> validators_count)
        //{
        //    UInt160 hash = new UInt160(descriptor.Key);
        //    AccountState account = accounts.GetAndChange(hash, () => new AccountState(hash));
        //    switch (descriptor.Field)
        //    {
        //        case "Votes":
        //            Fixed8 balance = account.GetBalance(GoverningToken.Hash);
        //            foreach (ECPoint pubkey in account.Votes)
        //            {
        //                ValidatorState validator = validators.GetAndChange(pubkey);
        //                validator.Votes -= balance;
        //                if (!validator.Registered && validator.Votes.Equals(Fixed8.Zero))
        //                    validators.Delete(pubkey);
        //            }
        //            ECPoint[] votes = descriptor.Value.AsSerializableArray<ECPoint>().Distinct().ToArray();
        //            if (votes.Length != account.Votes.Length)
        //            {
        //                ValidatorsCountState count_state = validators_count.GetAndChange();
        //                if (account.Votes.Length > 0)
        //                    count_state.Votes[account.Votes.Length - 1] -= balance;
        //                if (votes.Length > 0)
        //                    count_state.Votes[votes.Length - 1] += balance;
        //            }
        //            account.Votes = votes;
        //            foreach (ECPoint pubkey in account.Votes)
        //                validators.GetAndChange(pubkey, () => new ValidatorState(pubkey)).Votes += balance;
        //            break;
        //    }
        //}

        //protected void ProcessValidatorStateDescriptor(StateDescriptor descriptor, DataCache<ECPoint, ValidatorState> validators)
        //{
        //    ECPoint pubkey = ECPoint.DecodePoint(descriptor.Key, ECCurve.Secp256r1);
        //    ValidatorState validator = validators.GetAndChange(pubkey, () => new ValidatorState(pubkey));
        //    switch (descriptor.Field)
        //    {
        //        case "Registered":
        //            validator.Registered = BitConverter.ToBoolean(descriptor.Value, 0);
        //            break;
        //    }
        //}

        public void Dispose()
        {
        }
    }
}