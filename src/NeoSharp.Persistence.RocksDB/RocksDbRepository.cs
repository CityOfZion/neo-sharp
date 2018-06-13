using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Persistence;
using RocksDbSharp;

namespace NeoSharp.Persistence.RocksDB
{
    public abstract class RocksDbRepository<TEntity> : IRepository<TEntity> 
        where TEntity : NeoEntityBase, new()
    {
        #region Private Fields 
        private readonly IRepositoryConfiguration _config;
        private readonly IBinarySerializer _serializer;
        private readonly IBinaryDeserializer _deserializer;

        private RocksDb _rocksDbConnectionHandler;
        #endregion

        #region Constructor 
        protected RocksDbRepository(
            IRepositoryConfiguration config,
            IBinarySerializer serializer,
            IBinaryDeserializer deserializer)
        {
            _config = config;
            _serializer = serializer;
            _deserializer = deserializer;
        }
        #endregion

        #region IRepository Implementation 
        public void Create(TEntity entity)
        {
            CheckAndCreateIfNecessaryConnectionHandler();

            var hash = entity.Hash.ToArray();
            _rocksDbConnectionHandler.Put(hash.BuildKey(DataEntryPrefix.DataBlock), _serializer.Serialize(entity));
        }

        public void Delete(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public void Update(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public TEntity GetById(byte[] id)
        {
            CheckAndCreateIfNecessaryConnectionHandler();

            var rawId = _rocksDbConnectionHandler.Get(id.BuildKey(DataEntryPrefix.DataBlock));
            return _deserializer.Deserialize<TEntity>(rawId);
        }

        public ReadOnlyCollection<TEntity> GetAll()
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<TEntity> GetAll(Expression<Func<TEntity, bool>> expression)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Private Methods
        private void CheckAndCreateIfNecessaryConnectionHandler()
        {
            if (_rocksDbConnectionHandler != null) return;

            var options = new DbOptions().SetCreateIfMissing();
            _rocksDbConnectionHandler = RocksDb.Open(options, _config.ConnectionString);
        }
        #endregion
    }
}
