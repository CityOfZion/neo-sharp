using System;
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
        public UInt256 GetBlockHashFromHeight(uint height)
        {
            var hash = this._rocksDbContext.Get(BuildKey(DataEntryPrefix.IxHeightToHash, BitConverter.GetBytes(height)));
            return hash == null ? UInt256.Zero : new UInt256(hash);
        }

        public void AddBlockHeader(BlockHeaderBase blockHeader)
        {
            var hash = blockHeader.Hash.ToArray();
            var ix = BitConverter.GetBytes(blockHeader.Index);

            this._rocksDbContext.Save(BuildKey(DataEntryPrefix.DataBlock, hash), _serializer.Serialize(blockHeader));
            this._rocksDbContext.Save(BuildKey(DataEntryPrefix.IxHeightToHash, ix), hash);
        }

        public void AddTransaction(Transaction transaction)
        {
            var hash = transaction.Hash.ToArray();
            this._rocksDbContext.Save(BuildKey(DataEntryPrefix.DataTransaction, hash), _serializer.Serialize(transaction));
        }

        public BlockHeader GetBlockHeader(UInt256 hash)
        {
            var rawHeader = this._rocksDbContext.Get(BuildKey(DataEntryPrefix.DataBlock, hash.ToArray()));
            return rawHeader == null ? null : this._deserializer.Deserialize<BlockHeader>(rawHeader);
        }

        public uint GetTotalBlockHeight()
        {
            var raw = this._rocksDbContext.Get(this._sysCurrentBlockKey);
            return raw == null ? uint.MinValue : BitConverter.ToUInt32(raw, 0);
        }

        public void SetTotalBlockHeight(uint height)
        {
            this._rocksDbContext.Save(this._sysCurrentBlockKey, BitConverter.GetBytes(height));
        }

        public Transaction GetTransaction(UInt256 hash)
        {
            var bytes = this._rocksDbContext.Get(BuildKey(DataEntryPrefix.DataTransaction, hash.ToArray()));
            return bytes == null ? null : this._deserializer.Deserialize<Transaction>(bytes);
        }
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            this._rocksDbContext.Dispose();
        }
        #endregion

        #region Private Methods 
        /// <summary>
        /// Builds the concatenated key based on data type and desired key
        /// </summary>
        /// <param name="type">Data type</param>
        /// <param name="key">Desired key</param>
        /// <returns>Resulting key</returns>
        private static byte[] BuildKey(DataEntryPrefix type, byte[] key)
        {
            var len = key.Length;
            var bytes = new byte[len + 1];

            bytes[0] = (byte)type;
            Array.Copy(key, 0, bytes, 1, len);

            return bytes;
        }
        #endregion
    }
}
