using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RocksDbSharp;

namespace NeoSharp.Persistence.RocksDB
{
    public class RocksDbContext : IRocksDbContext
    {
        #region Private Fields

        private readonly RocksDb _rocksDb;

        #endregion

        #region Constructor

        public RocksDbContext(RocksDbConfig config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            // Initialize RocksDB (Connection String is the path to use)
            var options = new DbOptions().SetCreateIfMissing();
            // TODO #358: please avoid sync IO in constructor -> Open connection with the first operation for now
            _rocksDb = RocksDb.Open(options, config.FilePath);
        }

        #endregion

        #region IRocksDbContext implementation

        public Task<byte[]> Get(byte[] key)
        {
            return Task.FromResult(_rocksDb.Get(key));
        }

        public Task<IDictionary<byte[], byte[]>> GetMany(IEnumerable<byte[]> keys)
        {
            return Task.FromResult<IDictionary<byte[], byte[]>>(_rocksDb.MultiGet(keys.ToArray())
                .ToDictionary(kv => kv.Key, k => k.Value));
        }

        public Task Save(byte[] key, byte[] content)
        {
            _rocksDb.Put(key, content);

            return Task.CompletedTask;
        }

        public Task Delete(byte[] key)
        {
            _rocksDb.Remove(key);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _rocksDb?.Dispose();
        }

        #endregion
    }
}