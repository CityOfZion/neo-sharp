using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.Types;

namespace NeoSharp.Persistence.RocksDB
{
    public class RocksDbRepository : IRepository, IDisposable
    {
        #region Private Fields

        private readonly IRocksDbContext _rocksDbContext;
        private readonly IBinarySerializer _binarySerializer;

        private readonly byte[] _sysCurrentBlockKey = {(byte) DataEntryPrefix.SysCurrentBlock};
        private readonly byte[] _sysCurrentBlockHeaderKey = {(byte) DataEntryPrefix.SysCurrentHeader};
        private readonly byte[] _sysVersionKey = {(byte) DataEntryPrefix.SysVersion};
        private readonly byte[] _indexHeightKey = {(byte) DataEntryPrefix.IxIndexHeight};

        #endregion

        #region Constructor

        public RocksDbRepository
        (
            IRocksDbContext rocksDbContext,
            IBinarySerializer binarySerializer
        )
        {
            _rocksDbContext = rocksDbContext ?? throw new ArgumentNullException(nameof(rocksDbContext));
            _binarySerializer = binarySerializer ?? throw new ArgumentNullException(nameof(binarySerializer));
        }

        #endregion

        #region IRepository System Members

        public async Task<uint> GetTotalBlockHeight()
        {
            var raw = await _rocksDbContext.Get(_sysCurrentBlockKey);
            return raw == null ? uint.MinValue : BitConverter.ToUInt32(raw, 0);
        }

        public async Task SetTotalBlockHeight(uint height)
        {
            await _rocksDbContext.Save(_sysCurrentBlockKey, BitConverter.GetBytes(height));
        }

        public async Task<uint> GetTotalBlockHeaderHeight()
        {
            var raw = await _rocksDbContext.Get(_sysCurrentBlockHeaderKey);
            return raw == null ? uint.MinValue : BitConverter.ToUInt32(raw, 0);
        }

        public async Task SetTotalBlockHeaderHeight(uint height)
        {
            await _rocksDbContext.Save(_sysCurrentBlockHeaderKey, BitConverter.GetBytes(height));
        }

        public async Task<string> GetVersion()
        {
            var raw = await _rocksDbContext.Get(_sysVersionKey);
            return raw == null ? null : _binarySerializer.Deserialize<string>(raw);
        }

        public async Task SetVersion(string version)
        {
            await _rocksDbContext.Save(_sysVersionKey, _binarySerializer.Serialize(version));
        }

        #endregion

        #region IRepository Data Members

        public async Task<UInt256> GetBlockHashFromHeight(uint height)
        {
            var hash = await this._rocksDbContext.Get(height.BuildIxHeightToHashKey());
            return hash == null || hash.Length == 0 ? UInt256.Zero : new UInt256(hash);
        }

        public async Task<IEnumerable<UInt256>> GetBlockHashesFromHeights(IEnumerable<uint> heights)
        {
            var heightsHashes = await this._rocksDbContext.GetMany(heights.Select(h => h.BuildIxHeightToHashKey()));
            return heightsHashes.Values.Where(h => h != null && h.Length == UInt256.Zero.Size).Select(h => new UInt256(h));
        }

        public async Task AddBlockHeader(BlockHeader blockHeader)
        {
            await _rocksDbContext.Save(blockHeader.Hash.BuildDataBlockKey(), _binarySerializer.Serialize(blockHeader));
            await _rocksDbContext.Save(blockHeader.Index.BuildIxHeightToHashKey(), blockHeader.Hash.ToArray());
        }

        public async Task AddTransaction(Transaction transaction)
        {
            await _rocksDbContext.Save(transaction.Hash.BuildDataTransactionKey(), _binarySerializer.Serialize(transaction));
        }

        public async Task<BlockHeader> GetBlockHeader(UInt256 hash)
        {
            var rawHeader = await _rocksDbContext.Get(hash.BuildDataBlockKey());
            return rawHeader == null ? null : _binarySerializer.Deserialize<BlockHeader>(rawHeader);
        }

        public async Task<Transaction> GetTransaction(UInt256 hash)
        {
            var rawTransaction = await _rocksDbContext.Get(hash.BuildDataTransactionKey());
            return rawTransaction == null ? null : _binarySerializer.Deserialize<Transaction>(rawTransaction);
        }

        #endregion

        #region IRepository State Members

        public async Task<Account> GetAccount(UInt160 hash)
        {
            var raw = await _rocksDbContext.Get(hash.BuildStateAccountKey());
            return raw == null
                ? null
                : _binarySerializer.Deserialize<Account>(raw);
        }

        public async Task AddAccount(Account acct)
        {
            await _rocksDbContext.Save(acct.ScriptHash.BuildStateAccountKey(), _binarySerializer.Serialize(acct));
        }

        public async Task DeleteAccount(UInt160 hash)
        {
            await _rocksDbContext.Delete(hash.BuildStateAccountKey());
        }

        public async Task<CoinState[]> GetCoinStates(UInt256 txHash)
        {
            var raw = await _rocksDbContext.Get(txHash.BuildStateCoinKey());
            return raw == null
                ? null
                : _binarySerializer.Deserialize<CoinState[]>(raw);
        }

        public async Task AddCoinStates(UInt256 txHash, CoinState[] coinstates)
        {
            await _rocksDbContext.Save(txHash.BuildStateCoinKey(), _binarySerializer.Serialize(coinstates));
        }

        public async Task DeleteCoinStates(UInt256 txHash)
        {
            await _rocksDbContext.Delete(txHash.BuildStateCoinKey());
        }

        public async Task<Validator> GetValidator(ECPoint publicKey)
        {
            var raw = await _rocksDbContext.Get(publicKey.BuildStateValidatorKey());
            return raw == null
                ? null
                : _binarySerializer.Deserialize<Validator>(raw);
        }

        public async Task AddValidator(Validator validator)
        {
            await _rocksDbContext.Save(validator.PublicKey.BuildStateValidatorKey(), _binarySerializer.Serialize(validator));
        }

        public async Task DeleteValidator(ECPoint point)
        {
            await _rocksDbContext.Delete(point.BuildStateValidatorKey());
        }

        public async Task<Asset> GetAsset(UInt256 assetId)
        {
            var raw = await _rocksDbContext.Get(assetId.BuildStateAssetKey());
            return raw == null ? null : _binarySerializer.Deserialize<Asset>(raw);
        }

        public async Task AddAsset(Asset asset)
        {
            await _rocksDbContext.Save(asset.Id.BuildStateAssetKey(), _binarySerializer.Serialize(asset));
        }

        public async Task DeleteAsset(UInt256 assetId)
        {
            await _rocksDbContext.Delete(assetId.BuildStateAssetKey());
        }

        public async Task<Contract> GetContract(UInt160 contractHash)
        {
            var raw = await _rocksDbContext.Get(contractHash.BuildStateContractKey());
            return raw == null
                ? null
                : _binarySerializer.Deserialize<Contract>(raw);
        }

        public async Task AddContract(Contract contract)
        {
            await _rocksDbContext.Save(contract.ScriptHash.BuildStateContractKey(), _binarySerializer.Serialize(contract));
        }

        public async Task DeleteContract(UInt160 contractHash)
        {
            await _rocksDbContext.Delete(contractHash.BuildStateContractKey());
        }

        public async Task<StorageValue> GetStorage(StorageKey key)
        {
            var raw = await _rocksDbContext.Get(key.BuildStateStorageKey());
            return raw == null
                ? null
                : _binarySerializer.Deserialize<StorageValue>(raw);
        }

        public async Task AddStorage(StorageKey key, StorageValue val)
        {
            await _rocksDbContext.Save(key.BuildStateStorageKey(), val.Value);
        }

        public async Task DeleteStorage(StorageKey key)
        {
            await _rocksDbContext.Delete(key.BuildStateStorageKey());
        }

        #endregion

        #region IRepository Index Members

        public async Task<uint> GetIndexHeight()
        {
            var raw = await _rocksDbContext.Get(_indexHeightKey);
            return raw == null ? uint.MinValue : BitConverter.ToUInt32(raw, 0);
        }

        public async Task SetIndexHeight(uint height)
        {
            await _rocksDbContext.Save(_indexHeightKey, BitConverter.GetBytes(height));
        }

        public async Task<HashSet<CoinReference>> GetIndexConfirmed(UInt160 hash)
        {
            var raw = await _rocksDbContext.Get(hash.BuildIndexConfirmedKey());
            return raw == null
                ? new HashSet<CoinReference>()
                : _binarySerializer.Deserialize<HashSet<CoinReference>>(raw);
        }

        public async Task SetIndexConfirmed(UInt160 hash, HashSet<CoinReference> coinReferences)
        {
            var bytes = _binarySerializer.Serialize(coinReferences);
            await _rocksDbContext.Save(hash.BuildIndexConfirmedKey(), bytes);
        }

        public async Task<HashSet<CoinReference>> GetIndexClaimable(UInt160 hash)
        {
            var raw = await _rocksDbContext.Get(hash.BuildIndexClaimableKey());
            return raw == null
                ? new HashSet<CoinReference>()
                : _binarySerializer.Deserialize<HashSet<CoinReference>>(raw);
        }

        public async Task SetIndexClaimable(UInt160 hash, HashSet<CoinReference> coinReferences)
        {
            var bytes = _binarySerializer.Serialize(coinReferences);
            await _rocksDbContext.Save(hash.BuildIndexClaimableKey(), bytes);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _rocksDbContext.Dispose();
        }

        #endregion
    }
}
