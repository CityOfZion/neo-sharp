using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.Persistence.RedisDB.Helpers;
using NeoSharp.Types;
using StackExchange.Redis;

namespace NeoSharp.Persistence.RedisDB
{
    public class RedisDbJsonRepository : IRepository
    {
        #region Private Fields

        private readonly IRedisDbJsonContext _redisDbJsonContext;
        private readonly IJsonConverter _jsonConverter;

        private readonly string _sysCurrentBlockKey = DataEntryPrefix.SysCurrentBlock.ToString();
        private readonly string _sysCurrentBlockHeaderKey = DataEntryPrefix.SysCurrentHeader.ToString();
        private readonly string _sysVersionKey = DataEntryPrefix.SysVersion.ToString();
        private readonly string _indexHeightKey = DataEntryPrefix.IxIndexHeight.ToString();

        #endregion

        #region Constructor

        public RedisDbJsonRepository
        (
            IRedisDbJsonContext redisDbJsonContext
            //IJsonConverter jsonConverter
        )
        {
            _redisDbJsonContext = redisDbJsonContext ?? throw new ArgumentNullException(nameof(redisDbJsonContext));
            _jsonConverter = new JsonConverter(); //jsonConverter ?? throw new ArgumentNullException(nameof(jsonConverter));
        }

        #endregion

        #region IRepository System Members

        public async Task<uint> GetTotalBlockHeight()
        {
            var val = await _redisDbJsonContext.Get(_sysCurrentBlockKey);
            return val == RedisValue.Null ? uint.MinValue : (uint)val;
        }

        public async Task SetTotalBlockHeight(uint height)
        {
            await _redisDbJsonContext.Set(_sysCurrentBlockKey, height);
        }

        public async Task<uint> GetTotalBlockHeaderHeight()
        {
            var val = await _redisDbJsonContext.Get(_sysCurrentBlockHeaderKey);
            return val == RedisValue.Null ? uint.MinValue : (uint)val;
        }

        public async Task SetTotalBlockHeaderHeight(uint height)
        {
            await _redisDbJsonContext.Set(_sysCurrentBlockHeaderKey, height);
        }

        public async Task<string> GetVersion()
        {
            var val = await _redisDbJsonContext.Get(_sysVersionKey);
            return val == RedisValue.Null ? null : (string)val;
        }

        public async Task SetVersion(string version)
        {
            await _redisDbJsonContext.Set(_sysVersionKey, version);
        }

        #endregion

        #region IRepository Data Members

        public async Task AddBlockHeader(BlockHeader blockHeader)
        {
            var blockHeaderJson = _jsonConverter.SerializeObject(blockHeader);
            await _redisDbJsonContext.Set(blockHeader.Hash.BuildDataBlockKey(), blockHeaderJson);

            await _redisDbJsonContext.AddToIndex(RedisIndex.BlockTimestamp, blockHeader.Hash, blockHeader.Timestamp);
            await _redisDbJsonContext.AddToIndex(RedisIndex.BlockHeight, blockHeader.Hash, blockHeader.Index);
        }

        public async Task AddTransaction(Transaction transaction)
        {
            var transactionJson = _jsonConverter.SerializeObject(transaction);
            await _redisDbJsonContext.Set(transaction.Hash.BuildDataTransactionKey(), transactionJson);
        }

        public async Task<UInt256> GetBlockHashFromHeight(uint height)
        {
            var hash = await _redisDbJsonContext.GetFromHashIndex(RedisIndex.BlockHeight, height);
            return hash ?? UInt256.Zero;
        }

        public async Task<BlockHeader> GetBlockHeader(UInt256 hash)
        {
            var blockHeaderRedisValue = await _redisDbJsonContext.Get(hash.BuildDataBlockKey());
            return blockHeaderRedisValue.IsNull ? null : _jsonConverter.DeserializeObject<BlockHeader>(blockHeaderRedisValue);

        }

        public async Task<Transaction> GetTransaction(UInt256 hash)
        {
            var transactionRedisValue = await _redisDbJsonContext.Get(hash.BuildDataTransactionKey());
            return transactionRedisValue.IsNull ? null : _jsonConverter.DeserializeObject<Transaction>(transactionRedisValue);
        }

        #endregion

        #region IRepository State Members

        public async Task<Account> GetAccount(UInt160 hash)
        {
            var raw = await _redisDbJsonContext.Get(hash.BuildStateAccountKey());
            return raw.IsNull ? null : _jsonConverter.DeserializeObject<Account>(raw);
        }

        public async Task AddAccount(Account acct)
        {
            await _redisDbJsonContext.Set(acct.ScriptHash.BuildStateAccountKey(), _jsonConverter.SerializeObject(acct));
        }

        public async Task DeleteAccount(UInt160 hash)
        {
            await _redisDbJsonContext.Delete(hash.BuildStateAccountKey());
        }

        public async Task<CoinState[]> GetCoinStates(UInt256 txHash)
        {
            var raw = await _redisDbJsonContext.Get(txHash.BuildStateCoinKey());
            return raw.IsNull ? null : _jsonConverter.DeserializeObject<CoinState[]>(raw);
        }

        public async Task AddCoinStates(UInt256 txHash, CoinState[] coinStates)
        {
            await _redisDbJsonContext.Set(txHash.BuildStateCoinKey(), _jsonConverter.SerializeObject(coinStates));
        }

        public async Task DeleteCoinStates(UInt256 txHash)
        {
            await _redisDbJsonContext.Delete(txHash.BuildStateCoinKey());
        }

        public async Task<Validator> GetValidator(ECPoint publicKey)
        {
            var raw = await _redisDbJsonContext.Get(publicKey.BuildStateValidatorKey());
            return raw.IsNull ? null : _jsonConverter.DeserializeObject<Validator>(raw);
        }

        public async Task AddValidator(Validator validator)
        {
            await _redisDbJsonContext.Set(validator.PublicKey.BuildStateValidatorKey(), _jsonConverter.SerializeObject(validator));
        }

        public async Task DeleteValidator(ECPoint point)
        {
            await _redisDbJsonContext.Delete(point.BuildStateValidatorKey());
        }

        public async Task<Asset> GetAsset(UInt256 assetId)
        {
            var raw = await _redisDbJsonContext.Get(assetId.BuildStateAssetKey());
            return raw.IsNull ? null : _jsonConverter.DeserializeObject<Asset>(raw);
        }

        public async Task AddAsset(Asset asset)
        {
            await _redisDbJsonContext.Set(asset.Id.BuildStateAssetKey(), _jsonConverter.SerializeObject(asset));
        }

        public async Task DeleteAsset(UInt256 assetId)
        {
            await _redisDbJsonContext.Delete(assetId.BuildStateAssetKey());
        }

        public async Task<Contract> GetContract(UInt160 contractHash)
        {
            var raw = await _redisDbJsonContext.Get(contractHash.BuildStateContractKey());
            return raw.IsNull ? null : _jsonConverter.DeserializeObject<Contract>(raw);
        }

        public async Task AddContract(Contract contract)
        {
            await _redisDbJsonContext.Set(contract.ScriptHash.BuildStateContractKey(), _jsonConverter.SerializeObject(contract));
        }

        public async Task DeleteContract(UInt160 contractHash)
        {
            await _redisDbJsonContext.Delete(contractHash.BuildStateContractKey());
        }

        public async Task<StorageValue> GetStorage(StorageKey key)
        {
            var raw = await _redisDbJsonContext.Get(key.BuildStateStorageKey());
            return raw.IsNull ? null : _jsonConverter.DeserializeObject<StorageValue>(raw);
        }

        public async Task AddStorage(StorageKey key, StorageValue val)
        {
            await _redisDbJsonContext.Set(key.BuildStateStorageKey(), val.Value);
        }

        public async Task DeleteStorage(StorageKey key)
        {
            await _redisDbJsonContext.Delete(key.BuildStateStorageKey());
        }

        #endregion

        #region IRepository Index Members

        public async Task<uint> GetIndexHeight()
        {
            var val = await _redisDbJsonContext.Get(DataEntryPrefix.IxIndexHeight.ToString());
            return val.IsNull ? uint.MinValue : (uint) val;
        }

        public async Task SetIndexHeight(uint height)
        {
            await _redisDbJsonContext.Set(DataEntryPrefix.IxIndexHeight.ToString(), height);
        }

        public async Task<HashSet<CoinReference>> GetIndexConfirmed(UInt160 scriptHash)
        {
            var redisVal = await _redisDbJsonContext.Get(scriptHash.BuildIxConfirmedKey());
            return redisVal.IsNull ? new HashSet<CoinReference>() : _jsonConverter.DeserializeObject<HashSet<CoinReference>>(redisVal);
        }

        public async Task SetIndexConfirmed(UInt160 scriptHash, HashSet<CoinReference> coinReferences)
        {
            var json = _jsonConverter.SerializeObject(coinReferences);
            await _redisDbJsonContext.Set(scriptHash.BuildIxConfirmedKey(), json);
        }

        public async Task<HashSet<CoinReference>> GetIndexClaimable(UInt160 scriptHash)
        {
            var redisVal = await _redisDbJsonContext.Get(scriptHash.BuildIxClaimableKey());
            return redisVal.IsNull ? new HashSet<CoinReference>() : _jsonConverter.DeserializeObject<HashSet<CoinReference>>(redisVal);
        }

        public async Task SetIndexClaimable(UInt160 scriptHash, HashSet<CoinReference> coinReferences)
        {
            var json = _jsonConverter.SerializeObject(coinReferences.ToArray());
            await _redisDbJsonContext.Set(scriptHash.BuildIxClaimableKey(), json);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _redisDbJsonContext.Dispose();
        }

        #endregion
    }
}