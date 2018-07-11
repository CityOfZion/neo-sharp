using System;
using System.Collections.Generic;
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
            _redisDbContext = redisDbContext ?? throw new ArgumentNullException(nameof(redisDbContext));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
        }
        
        #endregion

        #region IRepository Members

        public async Task AddBlockHeader(BlockHeaderBase blockHeader)
        {
            if (_redisDbContext.IsBinaryMode)
            {
                var blockHeaderBytes = _serializer.Serialize(blockHeader);
                await _redisDbContext.Set(blockHeader.Hash.BuildDataBlockKey(), blockHeaderBytes);
            }
            else
            {
                // TODO [AboimPinto]: This serialization cannot be mocked, therefore cannot be tested properly.
                var blockHeaderJson = JsonConvert.SerializeObject(blockHeader);                         
                await _redisDbContext.Set(blockHeader.Hash.BuildDataBlockKey(), blockHeaderJson);
            }

            await _redisDbContext.AddToIndex(RedisIndex.BlockTimestamp, blockHeader.Hash, blockHeader.Timestamp);
            await _redisDbContext.AddToIndex(RedisIndex.BlockHeight, blockHeader.Hash, blockHeader.Index);
        }

        public async Task AddTransaction(Transaction transaction)
        {
            if (_redisDbContext.IsBinaryMode)
            {
                var transactionBytes = _serializer.Serialize(transaction);
                await _redisDbContext.Set(transaction.Hash.BuildDataTransactionKey(), transactionBytes);
            }
            else
            {
                var transactionJson = JsonConvert.SerializeObject(transaction);
                await _redisDbContext.Set(transaction.Hash.BuildDataTransactionKey(), transactionJson);
            }
        }

        public async Task<UInt256> GetBlockHashFromHeight(uint height)
        {
            return await _redisDbContext.GetFromHashIndex(RedisIndex.BlockHeight, height);
        }

        public async Task<BlockHeaderBase> GetBlockHeader(UInt256 hash)
        {
            var blockHeaderRedisValue = await _redisDbContext.Get(hash.BuildDataBlockKey());

            return _redisDbContext.IsBinaryMode ? 
                _deserializer.Deserialize<BlockHeaderBase>(blockHeaderRedisValue) : 
                JsonConvert.DeserializeObject<BlockHeaderBase>(blockHeaderRedisValue);
        }

        public async Task<BlockHeader> GetBlockHeaderExtended(UInt256 hash)
        {
            var blockHeaderRedisValue = await _redisDbContext.Get(hash.BuildDataBlockKey());

            return _redisDbContext.IsBinaryMode ?
                _deserializer.Deserialize<BlockHeader>(blockHeaderRedisValue) :
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
            var transactionRedisValue = await _redisDbContext.Get(hash.BuildDataTransactionKey());

            return _redisDbContext.IsBinaryMode ? 
                _deserializer.Deserialize<Transaction>(transactionRedisValue) : 
                JsonConvert.DeserializeObject<Transaction>(transactionRedisValue);
        }

        public Task<uint> GetIndexHeight()
        {
            throw new NotImplementedException();
        }

        public Task SetIndexHeight(uint height)
        {
            throw new NotImplementedException();
        }

        public Task<HashSet<CoinReference>> GetIndexConfirmed(UInt160 scriptHash)
        {
            throw new NotImplementedException();
        }

        public Task SetIndexConfirmed(UInt160 scriptHash, HashSet<CoinReference> coinReferences)
        {
            throw new NotImplementedException();
        }

        public Task<HashSet<CoinReference>> GetIndexClaimable(UInt160 scriptHash)
        {
            throw new NotImplementedException();
        }

        public Task SetIndexClaimable(UInt160 scriptHash, HashSet<CoinReference> coinReferences)
        {
            throw new NotImplementedException();
        }

        public Task<uint> GetTotalBlockHeaderHeight()
        {
            throw new NotImplementedException();
        }

        public Task SetTotalBlockHeaderHeight(uint height)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
