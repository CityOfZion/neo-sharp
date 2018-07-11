using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.Core.Types;

namespace NeoSharp.Persistence.RocksDB
{
    public class RocksDbRepository : IRepository, IDisposable
    {
        #region Private Fields

        private readonly IRocksDbContext _rocksDbContext;
        private readonly IBinarySerializer _serializer;
        private readonly IBinaryDeserializer _deserializer;

        private readonly byte[] _sysCurrentBlockKey = { (byte)DataEntryPrefix.SysCurrentBlock };
        private readonly byte[] _sysCurrentBlockHeaderKey = { (byte)DataEntryPrefix.SysCurrentHeader };

        #endregion

        #region Constructor

        public RocksDbRepository
            (
            IRocksDbContext rocksDbContext,
            IBinarySerializer serializer,
            IBinaryDeserializer deserializer
            )
        {
            _rocksDbContext = rocksDbContext ?? throw new ArgumentNullException(nameof(rocksDbContext));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
        }

        #endregion

        #region IRepository Members

        public async Task<UInt256> GetBlockHashFromHeight(uint height)
        {
            var hash = await _rocksDbContext.Get(height.BuildIxHeightToHashKey());
            return hash == null || hash.Length == 0 ? UInt256.Zero : new UInt256(hash);
        }

        public async Task AddBlockHeader(BlockHeaderBase blockHeader)
        {
            await _rocksDbContext.Save(blockHeader.Hash.BuildDataBlockKey(), _serializer.Serialize(blockHeader));
            await _rocksDbContext.Save(blockHeader.Index.BuildIxHeightToHashKey(), blockHeader.Hash.ToArray());
        }

        public async Task AddTransaction(Transaction transaction)
        {
            await _rocksDbContext.Save(transaction.Hash.BuildDataTransactionKey(), _serializer.Serialize(transaction));
        }

        public async Task<BlockHeaderBase> GetBlockHeader(UInt256 hash)
        {
            var rawHeader = await _rocksDbContext.Get(hash.BuildDataBlockKey());

            return rawHeader == null ? null : _deserializer.Deserialize<BlockHeaderBase>(rawHeader);
        }

        public async Task<BlockHeader> GetBlockHeaderExtended(UInt256 hash)
        {
            var rawHeader = await _rocksDbContext.Get(hash.BuildDataBlockKey());

            return rawHeader == null ? null : _deserializer.Deserialize<BlockHeader>(rawHeader);
        }

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

        public async Task<Transaction> GetTransaction(UInt256 hash)
        {
            var rawTransaction = await _rocksDbContext.Get(hash.BuildDataTransactionKey());

            return rawTransaction == null ? null : _deserializer.Deserialize<Transaction>(rawTransaction);
        }

        public Task<uint> GetIndexHeight()
        {
            throw new NotImplementedException();
        }

        public Task SetIndexHeight(uint height)
        {
            throw new NotImplementedException();
        }

        public Task<HashSet<CoinReference>> GetIndexConfirmed(UInt160 scriptHash)
        {
            throw new NotImplementedException();
        }

        public Task SetIndexConfirmed(UInt160 scriptHash, HashSet<CoinReference> coinReferences)
        {
            throw new NotImplementedException();
        }

        public Task<HashSet<CoinReference>> GetIndexClaimable(UInt160 scriptHash)
        {
            throw new NotImplementedException();
        }

        public Task SetIndexClaimable(UInt160 scriptHash, HashSet<CoinReference> coinReferences)
        {
            throw new NotImplementedException();
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
