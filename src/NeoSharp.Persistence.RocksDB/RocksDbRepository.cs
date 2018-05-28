using NeoSharp.BinarySerialization;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using RocksDbSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoSharp.Persistence.RocksDB
{
    public class RocksDbRepository : IRepository, IDisposable
    {
        private RocksDb _rocksDb;
        private readonly IBinarySerializer _serializer;
        private readonly IBinaryDeserializer _deserializer;

        public RocksDbRepository(IRepositoryConfiguration config, IBinarySerializer serializer, IBinaryDeserializer deserializer)
        {
            if(config == null)
                throw new ArgumentNullException(nameof(config));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));

            //Initialize RocksDB (Connection String is the path to use)
            var options = new DbOptions().SetCreateIfMissing(true);
            // TODO: please avoid sync IO in constructor -> Open connection with the first operation for now
            //_rocksDb = RocksDb.Open(options, config.ConnectionString);
        }

        #region IRepository Members
        public void AddBlockHeader(BlockHeader blockHeader)
        {
            var hash = blockHeader.Hash.ToArray();
            _rocksDb.Put(BuildKey(DataEntryPrefix.DataBlock,hash), _serializer.Serialize(blockHeader));
        }

        public void AddTransaction(Transaction transaction)
        {
            var hash = transaction.Hash.ToArray();
            _rocksDb.Put(BuildKey(DataEntryPrefix.DataTransaction,hash), _serializer.Serialize(transaction));
        }

        public BlockHeader GetBlockHeaderByHeight(int height)
        {
            throw new NotImplementedException();
        }

        public BlockHeader GetBlockHeaderById(byte[] id)
        {
            var bytes = GetRawBlockBytes(id);
            return _deserializer.Deserialize<BlockHeader>(bytes);
        }

        public BlockHeader GetBlockHeaderById(string id)
        {
            return GetBlockHeaderById(Encoding.UTF8.GetBytes(id));
        }

        public BlockHeader GetBlockHeaderByTimestamp(int timestamp)
        {
            throw new NotImplementedException();
        }

        public byte[] GetRawBlockBytes(string id)
        {
            return GetRawBlockBytes(Encoding.UTF8.GetBytes(id));
        }

        public byte[] GetRawBlockBytes(byte[] id)
        {
            return _rocksDb.Get(BuildKey(DataEntryPrefix.DataBlock,id));
        }

        public long GetTotalBlockHeight()
        {
            throw new NotImplementedException();
        }

        public Transaction GetTransaction(byte[] id)
        {
            var bytes = _rocksDb.Get(BuildKey(DataEntryPrefix.DataTransaction, id));
            return _deserializer.Deserialize<Transaction>(bytes);
        }

        public Transaction GetTransaction(string id)
        {
            return GetTransaction(Encoding.UTF8.GetBytes(id));
        }

        public Transaction[] GetTransactionsForBlock(byte[] id)
        {
            throw new NotImplementedException();
        }

        public Transaction[] GetTransactionsForBlock(string id)
        {
            throw new NotImplementedException();
        }
        #endregion

        /// <summary>
        /// Builds the concatenated key based on data type and desired key
        /// </summary>
        /// <param name="type">Data type</param>
        /// <param name="key">Desired key</param>
        /// <returns>Resulting key</returns>
        private byte[] BuildKey(DataEntryPrefix type, byte[] key)
        {
            List<byte> bytes = new List<byte>(key);
            bytes.Insert(0, (byte)type);
            return bytes.ToArray();
        }


        #region IDisposable Members
        public void Dispose()
        {
            if (_rocksDb != null)
                _rocksDb.Dispose();
        }
        #endregion
    }
}
