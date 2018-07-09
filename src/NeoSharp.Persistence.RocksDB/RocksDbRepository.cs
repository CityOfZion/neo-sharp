using System;
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
        #endregion

        #region Constructor
        public RocksDbRepository(
            IRocksDbContext rocksDbContext, 
            IBinarySerializer serializer, 
            IBinaryDeserializer deserializer)
        {
            this._rocksDbContext = rocksDbContext ?? throw new ArgumentNullException(nameof(rocksDbContext));
            this._serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            this._deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
        }
        #endregion

        #region IRepository Members
        public async Task<UInt256> GetBlockHashFromHeight(uint height)
        {
            var hash = await this._rocksDbContext.Get(height.BuildIxHeightToHashKey());
            return hash == null ? UInt256.Zero : new UInt256(hash);
        }

        public async Task AddBlockHeader(BlockHeaderBase blockHeader)
        {
            await this._rocksDbContext.Save(blockHeader.Hash.BuildDataBlockKey(), this._serializer.Serialize(blockHeader));
            await this._rocksDbContext.Save(blockHeader.Index.BuildIxHeightToHashKey(), blockHeader.Hash.ToArray());
        }

        public async Task AddTransaction(Transaction transaction)
        {
            await this._rocksDbContext.Save(transaction.Hash.BuildDataTransactionKey(), _serializer.Serialize(transaction));
        }

        public async Task<BlockHeader> GetBlockHeader(UInt256 hash)
        {
            var rawHeader = await this._rocksDbContext.Get(hash.BuildDataBlockKey());
            return rawHeader == null ? null : this._deserializer.Deserialize<BlockHeader>(rawHeader);
        }

        public async Task<uint> GetTotalBlockHeight()
        {
            var raw = await this._rocksDbContext.Get(this._sysCurrentBlockKey);
            return raw == null ? uint.MinValue : BitConverter.ToUInt32(raw, 0);
        }

        public async Task SetTotalBlockHeight(uint height)
        {
            await this._rocksDbContext.Save(this._sysCurrentBlockKey, BitConverter.GetBytes(height));
        }

        public async Task<Transaction> GetTransaction(UInt256 hash)
        {
            var rawTransaction = await this._rocksDbContext.Get(hash.BuildDataTransactionKey());
            return rawTransaction == null ? null : this._deserializer.Deserialize<Transaction>(rawTransaction);
        }
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            this._rocksDbContext.Dispose();
        }
        #endregion
    }
}
