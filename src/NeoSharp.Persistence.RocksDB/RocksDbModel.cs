using System;
using System.Threading.Tasks;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Persistence;
using NeoSharp.Core.Types;

namespace NeoSharp.Persistence.RocksDB
{
    public class RocksDbModel : IDbModel
    {
        #region Private Fields 
        private readonly IDbContext _dbContext;
        private readonly IBinarySerializer _serializer;
        private readonly IBinaryDeserializer _deserializer;
        #endregion

        #region Constructor 
        public RocksDbModel(
            IDbContext dbContext, 
            IBinarySerializer serializer, 
            IBinaryDeserializer deserializer)
        {
            _dbContext = dbContext;
            _serializer = serializer;
            _deserializer = deserializer;
        }
        #endregion

        #region IDbModel Implementation
        public Task Create<TEntity>(DataEntryPrefix dataEntryPrefix, UInt256 hash, TEntity entity) 
            where TEntity : Entity
        {
            var key = BuildKey(dataEntryPrefix, hash);
            var buffer = _serializer.Serialize(entity);

            return _dbContext.Create(key, buffer);
        }

        public Task Update<TEntity>(DataEntryPrefix dataEntryPrefix, UInt256 hash, TEntity entity) 
            where TEntity : Entity
        {
            var key = BuildKey(dataEntryPrefix, hash);

            throw new NotImplementedException();
        }

        public Task DeleteByHash(DataEntryPrefix dataEntryPrefix, UInt256 hash)
        {
            var key = BuildKey(dataEntryPrefix, hash);

            throw new NotImplementedException();
        }

        public async Task<TEntity> GetByHash<TEntity>(DataEntryPrefix dataEntryPrefix, UInt256 hash) 
            where TEntity : Entity
        {
            var key = BuildKey(dataEntryPrefix, hash);
            var buffer = await _dbContext.GetByHash(key);

            return (TEntity)_deserializer.Deserialize(buffer, typeof(TEntity));
        }
        #endregion

        private static byte[] BuildKey(DataEntryPrefix dataEntryPrefix, UInt256 hash)
        {
            return hash.BuildKey(dataEntryPrefix);
        }
    }
}
