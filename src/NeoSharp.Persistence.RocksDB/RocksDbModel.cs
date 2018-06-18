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
        public Task Create<TEntity>(TEntity entity, DataEntryPrefix dataEntryPrefix) 
            where TEntity : NeoEntityBase
        {
            var key = entity.Hash.BuildKey(dataEntryPrefix);
            var content = _serializer.Serialize(entity);

            return _dbContext.Create(key, content);
        }

        public Task Delete<TEntity>(TEntity entity) 
            where TEntity : NeoEntityBase
        {
            throw new NotImplementedException();
        }

        public Task Update<TEntity>(TEntity entity) 
            where TEntity : NeoEntityBase
        {
            throw new NotImplementedException();
        }

        public async Task<TEntity> GetByHash<TEntity>(UInt256 hash, DataEntryPrefix dataEntryPrefix) 
            where TEntity : NeoEntityBase
        {
            var rawBlockHeader = await _dbContext.GetByHash(hash.BuildKey(dataEntryPrefix));
            var deserializedBlockHeader = _deserializer.Deserialize(rawBlockHeader, typeof(TEntity)) as TEntity;

            return deserializedBlockHeader;
        }
        #endregion
    }
}
