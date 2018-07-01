using System;
using System.Collections.Generic;
using System.Text;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.Persistence.RedisDB.Helpers;
using Newtonsoft.Json;

namespace NeoSharp.Persistence.RedisDB
{
    public class RedisDbRepository : IRedisDbRepository
    {
        #region Private Fields 
        private readonly PersistenceConfig _persistenceConfig;
        private readonly IBinarySerializer _serializer;
        private readonly IBinaryDeserializer _deserializer;
        private readonly RedisHelper _redis;
        #endregion

        #region Construtor 
        public RedisDbRepository(
            PersistenceConfig persistenceConfig, 
            RedisDbConfig config, 
            IBinarySerializer serializer, 
            IBinaryDeserializer deserializer)
        {
            _persistenceConfig = persistenceConfig ?? throw new ArgumentNullException(nameof(persistenceConfig));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));

            var host = string.IsNullOrEmpty(config.ConnectionString) ? "localhost" : config.ConnectionString;
            var dbId = config.DatabaseId ?? 0;

            //Make the connection to the specified server and database
            if (_redis == null)
            {
                _redis = new RedisHelper(host, dbId);
            }
        }
        #endregion

        #region IRepository Members

        public void AddBlockHeader(BlockHeader blockHeader)
        {
            //Serialize
            if(_persistenceConfig.BinaryStorageProvider == BinaryStorageProvider.RedisDb)
            {
                var blockHeaderBytes = _serializer.Serialize(blockHeader);
                //Write the redis database with the binary bytes
                _redis.Database.Set(DataEntryPrefix.DataBlock, blockHeader.Hash.ToString(), blockHeaderBytes);
            }

            if (_persistenceConfig.JsonStorageProvider == JsonStorageProvider.RedisDb)
            {
                var blockHeaderJson = JsonConvert.SerializeObject(blockHeader);
                //Write the redis database with the binary bytes
                _redis.Database.Set(DataEntryPrefix.DataBlock, blockHeader.Hash.ToString(), blockHeaderJson);
            }
            
            //Add secondary indexes to find block hash by timestamp or height
            //Add to timestamp / blockhash index
            _redis.Database.AddToIndex(RedisIndex.BlockTimestamp, blockHeader.Timestamp, blockHeader.Hash.ToString());

            //Add to heignt / blockhash index
            _redis.Database.AddToIndex(RedisIndex.BlockHeight, blockHeader.Index, blockHeader.Hash.ToString());
        }

        public void AddTransaction(Transaction transaction)
        {
            if (_persistenceConfig.BinaryStorageProvider == BinaryStorageProvider.RedisDb)
            {
                //Convert to bytes
                var transactionBytes = _serializer.Serialize(transaction);
                //Write the redis database with the binary bytes
                _redis.Database.Set(DataEntryPrefix.DataTransaction, transaction.Hash.ToString(), transactionBytes);
            }

            if (_persistenceConfig.JsonStorageProvider == JsonStorageProvider.RedisDb)
            {
                //Convert to bytes
                var transactionJson = JsonConvert.SerializeObject(transaction);
                //Write the redis database with the binary bytes
                _redis.Database.Set(DataEntryPrefix.DataTransaction, transaction.Hash.ToString(), transactionJson);
            }
        }

        public BlockHeader GetBlockHeaderByHeight(int height)
        {
            //Get the hash for the block at the specified height from our secondary index
            var values = _redis.Database.GetFromIndex(RedisIndex.BlockHeight, height);

            //We want only the first result
            if (values.Length > 0)
                return GetBlockHeaderById(values[0]);

            return null;
        }

        public BlockHeader GetBlockHeaderByTimestamp(int timestamp)
        {
            //Get the hash for the block with the specified timestamp from our secondary index
            var values = _redis.Database.GetFromIndex(RedisIndex.BlockTimestamp, timestamp);

            //We want only the first result
            if (values.Length > 0)
                return GetBlockHeaderById(values[0]);

            return null;
        }

        public BlockHeader GetBlockHeaderById(byte[] id)
        {
            return GetBlockHeaderById(Encoding.UTF8.GetString(id));
        }

        public BlockHeader GetBlockHeaderById(string id)
        {
            //Retrieve the block header
            var blockHeader = GetRawBlock(id);

            if (_persistenceConfig.BinaryStorageProvider == BinaryStorageProvider.RedisDb)
            {
                return _deserializer.Deserialize<BlockHeader>((byte[])blockHeader);
            }

            if (_persistenceConfig.JsonStorageProvider == JsonStorageProvider.RedisDb)
            {
                return JsonConvert.DeserializeObject<BlockHeader>((string)blockHeader);
            }

            return null;
        }

        public object GetRawBlock(byte[] id)
        {
            return GetRawBlock(Encoding.UTF8.GetString(id));
        }

        public object GetRawBlock(string id)
        {
            return _redis.Database.Get(DataEntryPrefix.DataBlock, id);
        }

        public long GetTotalBlockHeight()
        {
            //Use the block height secondary index to tell us what our block height is
            return _redis.Database.GetIndexLength(RedisIndex.BlockHeight);
        }

        public Transaction GetTransaction(byte[] id)
        {
            return GetTransaction(Encoding.UTF8.GetString(id));
        }

        public Transaction GetTransaction(string id)
        {
            var transaction = _redis.Database.Get(DataEntryPrefix.DataTransaction, id);

            if (_persistenceConfig.BinaryStorageProvider == BinaryStorageProvider.RedisDb)
            {
                return _deserializer.Deserialize<Transaction>(transaction);
            }

            if (_persistenceConfig.JsonStorageProvider == JsonStorageProvider.RedisDb)
            {
                return JsonConvert.DeserializeObject<Transaction>(transaction);
            }

            return null;
        }

        public Transaction[] GetTransactionsForBlock(byte[] id)
        {
            return GetTransactionsForBlock(Encoding.UTF8.GetString(id));
        }

        public Transaction[] GetTransactionsForBlock(string id)
        {
            var transactions = new List<Transaction>();
            var block = GetBlockHeaderById(id);

            foreach (var transactionHash in block.TransactionHashes)
            {
                var transaction = GetTransaction(transactionHash.ToString());
                transactions.Add(transaction);
            }

            return transactions.ToArray();
        }
        #endregion
    }
}
