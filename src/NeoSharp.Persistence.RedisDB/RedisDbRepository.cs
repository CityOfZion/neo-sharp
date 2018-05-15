using NeoSharp.BinarySerialization;
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
        private readonly IBinarySerializer _serializer;
        private readonly IBinaryDeserializer _deserializer;
        private RedisHelper _redis;

        public RedisDbRepository(IBinarySerializer serializer, IBinaryDeserializer deserializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
        }

        #region IRepository Members

        public void AddBlockHeader(BlockHeader blockHeader)
        {
            //Convert to bytes
            var blockHeaderBytes = _serializer.Serialize(blockHeader);

            //Write the redis database with the binary bytes
            _redis.Database.Set(DataEntryPrefix.DataBlock, blockHeader.Hash.ToString(), blockHeaderBytes);

            //Add secondary indexes to find block hash by timestamp or height
            //Add to timestamp / blockhash index
            _redis.Database.AddToIndex(RedisIndex.BlockTimestamp, blockHeader.Timestamp, blockHeader.Hash.ToString());

            //Add to heignt / blockhash index
            _redis.Database.AddToIndex(RedisIndex.BlockHeight, blockHeader.Index, blockHeader.Hash.ToString());
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

        public BlockHeader GetBlockHeaderByHeight(int height)
        {
            //Get the hash for the block at the specified height from our secondary index
            RedisValue[] values = _redis.Database.GetFromIndex(RedisIndex.BlockHeight, height);

            //We want only the first result
            if(values.Length > 0)
                return GetBlockHeaderById((string)values[0]);

            return null;
        }

        public BlockHeader GetBlockHeaderByTimestamp(int timestamp)
        {
            //Get the hash for the block with the specified timestamp from our secondary index
            RedisValue[] values = _redis.Database.GetFromIndex(RedisIndex.BlockTimestamp, timestamp);

            //We want only the first result
            if (values.Length > 0)
                return GetBlockHeaderById((string)values[0]);

            return null;
        }

        public BlockHeader GetBlockHeaderById(byte[] id)
        {
            return GetBlockHeaderById(Encoding.UTF8.GetString(id));
        }

        public BlockHeader GetBlockHeaderById(string id)
        {
            //Retrieve the block header
            var blockHeaderBytes = GetRawBlockBytes(id);

            //Deserialize the block header
            return _deserializer.Deserialize<BlockHeader>(blockHeaderBytes);
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
            var block = GetBlockHeaderById(id);

            foreach(var transactionHash in block.TransactionHashes)
            {
                var transaction = GetTransaction(transactionHash);
                transactions.Add(transaction);
            }

            return transactions.ToArray();
        }

        public void Initialize(string connection, string database)
        {
            if (String.IsNullOrEmpty(connection))
                connection = "localhost";

            int.TryParse(database, out int dbId);

            //TODO: We need to make sure we persist this connection multiplexer, we don't want multiple connections within the app
            //Implementing now for testability, we will want to blend this into the repository / DI patterns
            _redis = new RedisHelper(ConnectionMultiplexer.Connect(connection), dbId);
        }

        #endregion
    }
}