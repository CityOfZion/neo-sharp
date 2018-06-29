using NeoSharp.BinarySerialization;
using NeoSharp.Core.Persistence;
using NeoSharp.Core.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NeoSharp.Persistence.RedisDB
{
    public class RedisDbBinaryModel : IDbModel
    {
        private readonly IDbBinaryContext _dbContext;
        private readonly IBinarySerializer _serializer;
        private readonly IBinaryDeserializer _deserializer;

        public RedisDbBinaryModel(
            IDbBinaryContext dbContext,
            IBinarySerializer serializer,
            IBinaryDeserializer deserializer)
        {
            _dbContext = dbContext;
            _serializer = serializer;
            _deserializer = deserializer;
        }

        public Task Create<TEntity>(DataEntryPrefix dataEntryPrefix, UInt256 hash, TEntity entity) where TEntity : Entity
        {
            var key = BuildKey(dataEntryPrefix, hash);
            var buffer = _serializer.Serialize(entity);

            return _dbContext.Create(key, buffer);
        }

        public Task Delete(DataEntryPrefix dataEntryPrefix, UInt256 hash)
        {
            var key = BuildKey(dataEntryPrefix, hash);
            return _dbContext.Delete(key);
        }

        public async Task<TEntity> Get<TEntity>(DataEntryPrefix dataEntryPrefix, UInt256 hash) where TEntity : Entity
        {
            var key = BuildKey(dataEntryPrefix, hash);
            var buffer = await _dbContext.Get(key);

            return _deserializer.Deserialize<TEntity>(buffer);
        }

        public Task Update<TEntity>(DataEntryPrefix dataEntryPrefix, UInt256 hash, TEntity entity) where TEntity : Entity
        {
            //Redis allows overwrites of key values without delete
            return Create(dataEntryPrefix, hash, entity);
        }

        private byte[] BuildKey(DataEntryPrefix dataEntryPrefix, UInt256 hash)
        {
            return hash.BuildKey(dataEntryPrefix);
        }
    }
}
