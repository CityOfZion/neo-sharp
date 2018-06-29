using System;
using System.Threading.Tasks;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Persistence;
using NeoSharp.Core.Types;

namespace NeoSharp.Persistence.RocksDB
{
    public class RocksDbBinaryModel : IDbModel
    {
        #region Private Fields 
        private readonly IDbBinaryContext _dbContext;
        private readonly IBinarySerializer _serializer;
        private readonly IBinaryDeserializer _deserializer;
        #endregion

        #region Constructor 
        public RocksDbBinaryModel(
            IDbBinaryContext dbContext, 
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
            var buffer = _serializer.Serialize(entity);

            //Delete first, does RocksDB allow overwrite?
            _dbContext.Delete(key);

            return _dbContext.Create(key, buffer);
        }

        public Task Delete(DataEntryPrefix dataEntryPrefix, UInt256 hash)
        {
            var key = BuildKey(dataEntryPrefix, hash);
            return _dbContext.Delete(key);
        }

        public async Task<TEntity> Get<TEntity>(DataEntryPrefix dataEntryPrefix, UInt256 hash) 
            where TEntity : Entity
        {
            var key = BuildKey(dataEntryPrefix, hash);
            var buffer = await _dbContext.Get(key);

            return (TEntity)_deserializer.Deserialize(buffer, typeof(TEntity));
        }
        #endregion

        private byte[] BuildKey(DataEntryPrefix dataEntryPrefix, UInt256 hash)
        {
            return hash.BuildKey(dataEntryPrefix);
        }
    }
}
