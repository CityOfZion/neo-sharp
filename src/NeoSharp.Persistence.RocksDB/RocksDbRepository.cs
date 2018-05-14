using NeoSharp.BinarySerialization.Interfaces;
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
        private readonly IBinarySerializer _serializer;
        private readonly IBinaryDeserializer _deserializer;

        public RocksDbRepository(IBinarySerializer serializer, IBinaryDeserializer deserializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
        }

        private RocksDb _rocksDb;

        #region IRepository Members
        public void AddBlock(Block block)
        {
            var hash = Encoding.UTF8.GetBytes(block.Hash);
            _rocksDb.Put(BuildKey(DataEntryPrefix.DataBlock,hash), _serializer.Serialize(block));
        }

        public void AddTransaction(Transaction transaction)
        {
            var hash = transaction.Hash.ToArray();
            _rocksDb.Put(BuildKey(DataEntryPrefix.DataTransaction,hash), _serializer.Serialize(transaction));
        }

        public Block GetBlockByHeight(int height)
        {
            throw new NotImplementedException();
        }

        public Block GetBlockById(byte[] id)
        {
            var bytes = GetRawBlockBytes(id);
            return _deserializer.Deserialize<Block>(bytes);
        }

        public Block GetBlockById(string id)
        {
            return GetBlockById(Encoding.UTF8.GetBytes(id));
        }

        public Block GetBlockByTimestamp(int timestamp)
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

        public void Initialize(string connection, string database)
        {
            if (String.IsNullOrEmpty(connection))
                throw new ArgumentNullException(nameof(connection), "No connection / path provided for RocksDB");

            //Connection = path in rocksDB, but I don't like this - we need to rethink how we could
            //Make this signature more generic for all of the varieties of repositories
            var options = new DbOptions().SetCreateIfMissing(true);
            _rocksDb = RocksDb.Open(options, connection);
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
