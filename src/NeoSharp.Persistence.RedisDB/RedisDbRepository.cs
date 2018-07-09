using System;
using System.Threading.Tasks;
using NeoSharp.BinarySerialization;
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
        private readonly IRedisDbContext _redisDbContext;
        private readonly IBinarySerializer _serializer;
        private readonly IBinaryDeserializer _deserializer;
        #endregion

        #region Construtor
        public RedisDbRepository(
            IRedisDbContext redisDbContext,
            IBinarySerializer serializer,
            IBinaryDeserializer deserializer)
        {
            this._redisDbContext = redisDbContext ?? throw new ArgumentNullException(nameof(redisDbContext));
            this._serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            this._deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
        }
        #endregion

        #region IRepository Members
        public async Task AddBlockHeader(BlockHeaderBase blockHeader)
        {
            if (this._redisDbContext.IsBinaryMode)
            {
                var blockHeaderBytes = this._serializer.Serialize(blockHeader);
                await this._redisDbContext.Set(blockHeader.Hash.BuildDataBlockKey(), blockHeaderBytes);
            }
            else
            {
                // TODO [AboimPinto]: This serialization cannot be mocked, therefore cannot be tested properly.
                var blockHeaderJson = JsonConvert.SerializeObject(blockHeader);                         
                await this._redisDbContext.Set(blockHeader.Hash.BuildDataBlockKey(), blockHeaderJson);
            }

            await this._redisDbContext.AddToIndex(RedisIndex.BlockTimestamp, blockHeader.Hash, blockHeader.Timestamp);
            await this._redisDbContext.AddToIndex(RedisIndex.BlockHeight, blockHeader.Hash, blockHeader.Index);
        }

        public async Task AddTransaction(Transaction transaction)
        {
            if (this._redisDbContext.IsBinaryMode)
            {
                var transactionBytes = _serializer.Serialize(transaction);
                await this._redisDbContext.Set(transaction.Hash.BuildDataTransactionKey(), transactionBytes);
            }
            else
            {
                var transactionJson = JsonConvert.SerializeObject(transaction);
                await this._redisDbContext.Set(transaction.Hash.BuildDataTransactionKey(), transactionJson);
            }
        }

        public async Task<UInt256> GetBlockHashFromHeight(uint height)
        {
            return await this._redisDbContext.GetFromHashIndex(RedisIndex.BlockHeight, height);
        }

        public async Task<BlockHeader> GetBlockHeaderByTimestamp(int timestamp)
        {
            var hash = await this._redisDbContext.GetFromHashIndex(RedisIndex.BlockTimestamp, timestamp);

            if (hash != null)
            {
                return await this.GetBlockHeader(hash);
            }

            return null;
        }

        public async Task<BlockHeader> GetBlockHeader(UInt256 hash)
        {
            var blockHeaderRedisValue = await this._redisDbContext.Get(hash.BuildDataBlockKey());

            return this._redisDbContext.IsBinaryMode ? 
                this._deserializer.Deserialize<BlockHeader>(blockHeaderRedisValue) : 
                JsonConvert.DeserializeObject<BlockHeader>(blockHeaderRedisValue);
        }

        public Task SetTotalBlockHeight(uint height)
        {
            throw new NotImplementedException();
            // TODO: redis logic
            //_redis.Database.AddToIndex(RedisIndex.BlockHeight, height);
        }

        public Task<uint> GetTotalBlockHeight()
        {
            //Use the block height secondary index to tell us what our block height is
            //return _redis.Database.GetIndexLength(RedisIndex.BlockHeight);

            // TODO: redis logic
            throw new NotImplementedException();
        }

        public async Task<Transaction> GetTransaction(UInt256 hash)
        {
            var transactionRedisValue = await this._redisDbContext.Get(hash.BuildDataTransactionKey());

            return this._redisDbContext.IsBinaryMode ? 
                this._deserializer.Deserialize<Transaction>(transactionRedisValue) : 
                JsonConvert.DeserializeObject<Transaction>(transactionRedisValue);
        }
        #endregion
    }
}
