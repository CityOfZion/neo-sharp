using System;
using System.Threading.Tasks;
using NeoSharp.Core.Persistence;
using RocksDbSharp;

namespace NeoSharp.Persistence.RocksDB
{
    public class RocksDbContext : IDbContext
    {
        #region Private Fields 
        private readonly IRepositoryConfiguration _config;

        private RocksDb _rocksDbConnectionHandler;
        #endregion

        #region Constructor 

        public RocksDbContext(IRepositoryConfiguration config)
        {
            _config = config;
        }
        
        #endregion

        #region IRepository Implementation 
        public Task Create(byte[] key, byte[] content)
        {
            var taskCompletionSource = new TaskCompletionSource<object>();

            Task.Factory.StartNew(() =>
            {
                CheckAndCreateIfNecessaryTheConnectionHandler();

                _rocksDbConnectionHandler.Put(key, content);
                taskCompletionSource.SetResult(null);
            });

            return taskCompletionSource.Task;
        }

        public Task Delete(byte[] key)
        {
            var taskCompletionSource = new TaskCompletionSource<object>();

            Task.Factory.StartNew(() =>
            {
                CheckAndCreateIfNecessaryTheConnectionHandler();

                _rocksDbConnectionHandler.Remove(key);
                taskCompletionSource.SetResult(null);
            });

            return taskCompletionSource.Task;
        }

        public Task Update(byte[] key, byte[] content)
        {
            // TODO [AboimPinto]: How the update is implemented? Like the Create or need to delete the Key and Create the new value?

            throw new NotImplementedException();
        }

        public Task<byte[]> GetByHash(byte[] hash)
        {
            var taskCompletionSource = new TaskCompletionSource<byte[]>();

            Task.Factory.StartNew(() =>
            {
                CheckAndCreateIfNecessaryTheConnectionHandler();

                var rawContent = _rocksDbConnectionHandler.Get(hash);
                taskCompletionSource.SetResult(rawContent);
            });

            return taskCompletionSource.Task;
        }
        #endregion

        #region Private Methods
        private void CheckAndCreateIfNecessaryTheConnectionHandler()
        {
            if (_rocksDbConnectionHandler != null) return;

            var options = new DbOptions().SetCreateIfMissing();
            _rocksDbConnectionHandler = RocksDb.Open(options, _config.ConnectionString);
        }
        #endregion
    }
}
