using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using NeoSharp.Core.Caching;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Blockchain
{
    public class Blockchain : IDisposable, IBlockchain
    {
        private readonly IRepository _neoSharpRepository;
        public static event EventHandler<Block> PersistCompleted;

        /// <summary>
        /// Memory pool
        /// </summary>
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

        public Blockchain(IRepository neoSharpRepository)
        {
            _neoSharpRepository = neoSharpRepository;

            LastBlockHeader = CurrentBlock = GetBlock(0);

            if (CurrentBlock == null)
            {
                AddBlock(Genesis.GenesisBlock);
            }
        }

        public Block CurrentBlock { get; private set; }

        public BlockHeader LastBlockHeader { get; private set; }

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

            _neoSharpRepository.AddBlockHeader(block);

            foreach (var tx in block.Transactions)
            {
                _neoSharpRepository.AddTransaction(tx);
            }

            return true;
        }

        /// <summary>
        /// Determine whether the specified block is contained in the blockchain
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public bool ContainsBlock(UInt256 hash)
        {
            return _neoSharpRepository.GetBlockHeader(hash.ToArray()) != null;
        }

        /// <summary>
        /// Return the corresponding block information according to the specified height
        /// </summary>
        /// <param name="height">Height</param>
        /// <returns></returns>
        public Block GetBlock(uint height)
        {
            UInt256 hash = GetBlockHash(height);
            if (hash == null) return null;
            return GetBlock(hash);
        }

        /// <summary>
        /// Return the corresponding block information according to the specified height
        /// </summary>
        /// <param name="hash">Hash</param>
        /// <returns></returns>
        public Block GetBlock(UInt256 hash)
        {
            var header = _neoSharpRepository.GetBlockHeader(hash.ToArray());

            if (header != null)
            {
                var txs = header.TransactionHashes.Select(u => _neoSharpRepository.GetTransaction(u.ToArray())).ToArray();

                return new Block()
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
                    Transactions = txs,
                };
            }

            return null;
        }

        public IEnumerable<Block> GetBlocks(IReadOnlyCollection<UInt256> blockHashes)
        {
            foreach (var hash in blockHashes)
            {
                var block = GetBlock(hash);

                if (block == null) continue;

                yield return block;
            }
        }

        /// <summary>
        /// Returns the hash of the corresponding block based on the specified height
        /// </summary>
        /// <param name="height"></param>
        /// <returns></returns>
        public UInt256 GetBlockHash(uint height)
        {
            var hash = _neoSharpRepository.GetBlockHashFromHeight(height);

            if (hash != null) return new UInt256(hash);

            return UInt256.Zero;
        }

        /// <summary>
        /// Returns the information for the next block based on the specified hash value
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public Block GetNextBlock(UInt256 hash)
        {
            var header = _neoSharpRepository.GetBlockHeader(hash.ToArray());

            if (header != null)
            {
                return GetBlock(header.Index + 1);
            }

            return null;
        }

        /// <summary>
        /// Returns the hash value of the next block based on the specified hash value
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public UInt256 GetNextBlockHash(UInt256 hash)
        {
            var header = _neoSharpRepository.GetBlockHeader(hash.ToArray());

            if (header != null)
            {
                var nextHash = _neoSharpRepository.GetBlockHashFromHeight(header.Index + 1);

                if (nextHash != null) return new UInt256(nextHash);
            }

            return UInt256.Zero;
        }

        #endregion

        #region BlockHeaders

        /// <summary>
        /// Add the specified block headers to the blockchain
        /// </summary>
        /// <param name="blockHeaders"></param>
        public void AddBlockHeaders(IEnumerable<BlockHeader> blockHeaders)
        {
            // TODO: hook up persistence here
            if (blockHeaders.Any())
            {
                LastBlockHeader = blockHeaders.OrderBy(h => h.Index).Last();
            }
        }

        /// <summary>
        /// Return the corresponding block header information according to the specified height
        /// </summary>
        /// <param name="height"></param>
        /// <returns></returns>
        public BlockHeader GetBlockHeader(uint height)
        {
            var hash = _neoSharpRepository.GetBlockHashFromHeight(height);

            if (hash != null) return _neoSharpRepository.GetBlockHeader(hash);

            return null;
        }

        /// <summary>
        /// Returns the corresponding block header information according to the specified hash value
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public BlockHeader GetBlockHeader(UInt256 hash)
        {
            return _neoSharpRepository.GetBlockHeader(hash.ToArray());
        }

        #endregion

        #region Transactions

        /// <summary>
        /// Returns the corresponding transaction information according to the specified hash value
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public Transaction GetTransaction(UInt256 hash)
        {
            return _neoSharpRepository.GetTransaction(hash.ToArray());
        }

        /// <summary>
        /// Return the corresponding transaction information and the height of the block where the transaction is located according to the specified hash value
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public Transaction GetTransaction(UInt256 hash, out int height)
        {
            // TODO: How to get the height?

            height = 0;
            return _neoSharpRepository.GetTransaction(hash.ToArray());
        }

        public IEnumerable<Transaction> GetTransactions(IReadOnlyCollection<UInt256> transactionHashes)
        {
            foreach (var hash in transactionHashes)
            {
                var tx = GetTransaction(hash);

                if (tx == null) continue;

                yield return tx;
            }
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

        /// <summary>
        /// Determine whether the specified transaction is included in the blockchain
        /// </summary>
        /// <param name="hash">Transaction hash</param>
        /// <returns>Return true if the specified transaction is included</returns>
        public bool ContainsTransaction(UInt256 hash)
        {
            return false;
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

        /// <summary>
        /// Return the corresponding contract information according to the specified hash
        /// </summary>
        /// <param name="hash">Hash</param>
        /// <returns></returns>
        public Contract GetContract(UInt256 hash)
        {
            return null;
        }

        /// <summary>
        /// Return the corresponding asset information according to the specified hash
        /// </summary>
        /// <param name="hash">Hash</param>
        /// <returns></returns>
        public Asset GetAsset(UInt256 hash)
        {
            return null;
        }

        /// <summary>
        /// Return all assets
        /// </summary>
        public IEnumerable<Asset> GetAssets()
        {
            yield break;
        }

        /// <summary>
        /// Return all contracts
        /// </summary>
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

        /// <summary>
        /// Returns the total amount of system costs contained in the corresponding block and all previous blocks based on the specified block height
        /// </summary>
        /// <param name="height"></param>
        /// <returns></returns>
        public virtual long GetSysFeeAmount(uint height)
        {
            return GetSysFeeAmount(GetBlockHash(height));
        }

        /// <summary>
        /// Returns the total amount of system charges contained in the corresponding block and all previous blocks based on the specified block hash value
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public long GetSysFeeAmount(UInt256 hash)
        {
            return 0;
        }

        //public Dictionary<ushort, SpentCoin> GetUnclaimed(UInt256 hash);

        /// <summary>
        /// Get the corresponding unspent assets based on the specified hash value and index
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="index"></param>
        /// <returns></returns>
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