using NeoSharp.BinarySerialization;
using NeoSharp.BinarySerialization.Interfaces;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoSharp.Persistence.RedisDB
{
    public class RedisDbRepository : IRepository
    {
        private RedisHelper _redis;

        private readonly IBinarySerializer _serializer;
        private readonly IBinaryDeserializer _deserializer;

        public RedisDbRepository(IBinarySerializer serializer, IBinaryDeserializer deserializer)
        {
            _serializer = serializer;
            _deserializer = deserializer;
        }

        #region IRepository Members

        public void AddBlock(Block block)
        {
            //Convert to bytes
            var blockBytes = _serializer.Serialize(block);

            //Write the redis database with the binary bytes
            _redis.Database.Set(DataEntryPrefix.DataBlock, block.Hash, blockBytes);

            //Add secondary indexes to find block hash by timestamp or height
            //Add to timestamp / blockhash index
            _redis.Database.AddToIndex(RedisIndex.BlockTimestamp, block.Timestamp, block.Hash);

            //Add to heignt / blockhash index
            _redis.Database.AddToIndex(RedisIndex.BlockHeight, block.Index, block.Hash);
        }

        public void AddTransaction(Transaction transaction)
        {
            //Convert to bytes
            var transactionBytes = _serializer.Serialize(transaction);

            //Write the redis database with the binary bytes
            _redis.Database.Set(DataEntryPrefix.DataTransaction, transaction.Hash.ToString(), transactionBytes);

            //Add to secondary index to find transactions for a block by block height
            _redis.Database.AddToIndex(RedisIndex.TransactionBlockHeight, transaction.BlockIndex, transaction.Hash.ToString());
        }

        public Block GetBlockByHeight(int height)
        {
            //Get the hash for the block at the specified height from our secondary index
            RedisValue[] values = _redis.Database.GetFromIndex(RedisIndex.BlockHeight, height);

            //We want only the first result
            if(values.Length > 0)
                return GetBlockById((string)values[0]);

            return null;
        }

        public Block GetBlockByTimestamp(int timestamp)
        {
            //Get the hash for the block with the specified timestamp from our secondary index
            RedisValue[] values = _redis.Database.GetFromIndex(RedisIndex.BlockTimestamp, timestamp);

            //We want only the first result
            if (values.Length > 0)
                return GetBlockById((string)values[0]);

            return null;
        }

        public Block GetBlockById(byte[] id)
        {
            return GetBlockById(Encoding.UTF8.GetString(id));
        }

        public Block GetBlockById(string id)
        {
            //Retrieve the block
            var blockBytes = GetRawBlockBytes(id);

            //Deserialize the block
            return _deserializer.Deserialize<Block>(blockBytes);
        }

        public byte[] GetRawBlockBytes(string id)
        {
            return _redis.Database.Get(DataEntryPrefix.DataBlock, id);
        }

        public byte[] GetRawBlockBytes(byte[] id)
        {
            return GetRawBlockBytes(Encoding.UTF8.GetString(id));
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
            var transactionBytes = _redis.Database.Get(DataEntryPrefix.DataTransaction, id);
            return _deserializer.Deserialize<Transaction>(transactionBytes);
        }

        public Transaction[] GetTransactionsForBlock(byte[] id)
        {
            return GetTransactionsForBlock(Encoding.UTF8.GetString(id));
        }

        public Transaction[] GetTransactionsForBlock(string id)
        {
            //TODO:  This can be optimized
            List<Transaction> transactions = new List<Transaction>();
            var block = GetBlockById(id);

            foreach(var txHash in block.TxHashes)
            {
                var tx = GetTransaction(txHash);
                transactions.Add(tx);
            }

            return transactions.ToArray();
        }

        public void Initialize(string connection, string database)
        {
            if (String.IsNullOrEmpty(connection))
                connection = "localhost";

            int dbId = 0;
            int.TryParse(database, out dbId);

            //TODO: We need to make sure we persist this connection multiplexer, we don't want multiple connections within the app
            //Implementing now for testability, we will want to blend this into the repository / DI patterns
            _redis = new RedisHelper(ConnectionMultiplexer.Connect(connection), dbId);
        }

        #endregion
    }
}