using System;
using System.Collections.Generic;
using System.Text;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Extensions;
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

            if (this._persistenceConfig.BinaryStorageProvider == BinaryStorageProvider.RedisDb ||
                this._persistenceConfig.JsonStorageProvider == JsonStorageProvider.RedisDb)
            {
                //Make the connection to the specified server and database
                if (_redis == null)
                {
                    _redis = new RedisHelper(host, dbId);
                }
            }
        }
        #endregion

        #region IRepository Members

        public void AddBlockHeader(BlockHeader blockHeader)
        {
            //Serialize
            if (_persistenceConfig.BinaryStorageProvider == BinaryStorageProvider.RedisDb)
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

        public byte[] GetBlockHashFromHeight(uint height)
        {
            //Get the hash for the block at the specified height from our secondary index
            var values = _redis.Database.GetFromIndex(RedisIndex.BlockHeight, height);

            //We want only the first result
            if (values.Length > 0)
                return values[0].HexToBytes();

            return null;
        }

        public BlockHeader GetBlockHeaderByTimestamp(int timestamp)
        {
            //Get the hash for the block with the specified timestamp from our secondary index
            var values = _redis.Database.GetFromIndex(RedisIndex.BlockTimestamp, timestamp);

            //We want only the first result
            if (values.Length > 0)
                return GetBlockHeader(values[0].HexToBytes());

            return null;
        }

        public BlockHeader GetBlockHeader(byte[] hash)
        {
            //Retrieve the block header
            var blockHeader = _redis.Database.Get(DataEntryPrefix.DataBlock, hash);

            if (_persistenceConfig.BinaryStorageProvider == BinaryStorageProvider.RedisDb)
            {
                return _deserializer.Deserialize<BlockHeader>(blockHeader);
            }

            if (_persistenceConfig.JsonStorageProvider == JsonStorageProvider.RedisDb)
            {
                return JsonConvert.DeserializeObject<BlockHeader>(blockHeader);
            }

            return null;
        }

        public long GetTotalBlockHeight()
        {
            //Use the block height secondary index to tell us what our block height is
            return _redis.Database.GetIndexLength(RedisIndex.BlockHeight);
        }

        public Transaction GetTransaction(byte[] hash)
        {
            var transaction = _redis.Database.Get(DataEntryPrefix.DataTransaction, hash);

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

        #endregion
    }
}
