using System;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.Core.Types;
using NeoSharp.Persistence.RedisDB.Helpers;
using Newtonsoft.Json;

namespace NeoSharp.Persistence.RedisDB
{
    public class RedisDbRepository : IRepository
    {
        #region Private Fields 

        private readonly RedisDbConfig _config;
        private readonly IBinarySerializer _serializer;
        private readonly IBinaryDeserializer _deserializer;
        private readonly RedisHelper _redis;

        #endregion

        #region Construtor

        public RedisDbRepository(
            RedisDbConfig config,
            IBinarySerializer serializer,
            IBinaryDeserializer deserializer)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
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

        public void AddBlockHeader(BlockHeaderBase blockHeader)
        {
            //Serialize
            if (_config.IsBinaryMode)
            {
                var blockHeaderBytes = _serializer.Serialize(blockHeader);
                //Write the redis database with the binary bytes
                _redis.Database.Set(DataEntryPrefix.DataBlock, blockHeader.Hash.ToString(), blockHeaderBytes);
            }
            else
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
            if (_config.IsBinaryMode)
            {
                //Convert to bytes
                var transactionBytes = _serializer.Serialize(transaction);
                //Write the redis database with the binary bytes
                _redis.Database.Set(DataEntryPrefix.DataTransaction, transaction.Hash.ToString(), transactionBytes);
            }
            else
            {
                //Convert to bytes
                var transactionJson = JsonConvert.SerializeObject(transaction);
                //Write the redis database with the binary bytes
                _redis.Database.Set(DataEntryPrefix.DataTransaction, transaction.Hash.ToString(), transactionJson);
            }
        }

        public UInt256 GetBlockHashFromHeight(uint height)
        {
            //Get the hash for the block at the specified height from our secondary index
            var values = _redis.Database.GetFromIndex(RedisIndex.BlockHeight, height);

            //We want only the first result
            return values.Length > 0 ? new UInt256(values[0].HexToBytes()) : null;
        }

        public BlockHeader GetBlockHeaderByTimestamp(int timestamp)
        {
            //Get the hash for the block with the specified timestamp from our secondary index
            var values = _redis.Database.GetFromIndex(RedisIndex.BlockTimestamp, timestamp);

            //We want only the first result
            return values.Length > 0 ? GetBlockHeader(new UInt256(values[0].HexToBytes())) : null;
        }

        public BlockHeader GetBlockHeader(UInt256 hash)
        {
            //Retrieve the block header
            var blockHeader = _redis.Database.Get(DataEntryPrefix.DataBlock, hash.ToArray());

            if (_config.IsBinaryMode)
            {
                return _deserializer.Deserialize<BlockHeader>(blockHeader);
            }
            else
            {
                return JsonConvert.DeserializeObject<BlockHeader>(blockHeader);
            }
        }

        public void SetTotalBlockHeight(uint height)
        {
            // TODO: redis logic
            //_redis.Database.AddToIndex(RedisIndex.BlockHeight, height);
        }

        public uint GetTotalBlockHeight()
        {
            //Use the block height secondary index to tell us what our block height is
            //return _redis.Database.GetIndexLength(RedisIndex.BlockHeight);

            // TODO: redis logic
            throw new NotImplementedException();
        }

        public Transaction GetTransaction(UInt256 hash)
        {
            var transaction = _redis.Database.Get(DataEntryPrefix.DataTransaction, hash.ToArray());

            if (_config.IsBinaryMode)
            {
                return _deserializer.Deserialize<Transaction>(transaction);
            }
            else
            {
                return JsonConvert.DeserializeObject<Transaction>(transaction);
            }
        }

        #endregion
    }
}
