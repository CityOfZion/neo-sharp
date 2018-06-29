using NeoSharp.Core.Persistence;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace NeoSharp.Persistence.RedisDB
{
    public class RedisDbBinaryContext : IDbBinaryContext
    {
        private readonly IRepositoryConfiguration _config;
        private readonly IDatabase _redisDb;

        public RedisDbBinaryContext(IRepositoryConfiguration config)
        {
            _config = config;

            //If no connection string provided, we can just default to localhost
            if (String.IsNullOrEmpty(config.ConnectionString))
                config.ConnectionString = "localhost";

            //Default to DB 0
            int dbId = config.DatabaseId == null ? 0 : (int)config.DatabaseId;

            //Make the connection to the specified server and database
            var redis = ConnectionMultiplexer.Connect(config.ConnectionString);

            //Retrieve the Redis DB with the specified number (by default, there are 16), default to 0.
            _redisDb = redis.GetDatabase(dbId);
        }

        public Task Create(byte[] key, byte[] content)
        {
            var taskCompletionSource = new TaskCompletionSource<object>();

            Task.Factory.StartNew(() =>
            {
                _redisDb.HashSet(key, new HashEntry[] { new HashEntry("data", content) });
                taskCompletionSource.SetResult(null);
            });

            return taskCompletionSource.Task;
        }

        public Task Delete(byte[] key)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> Get(byte[] key)
        {
            var taskCompletionSource = new TaskCompletionSource<byte[]>();

            Task.Factory.StartNew(() =>
            {
                var rawContent = _redisDb.HashGet(key, "data");
                taskCompletionSource.SetResult(rawContent);
            });

            return taskCompletionSource.Task;
        }

        public Task Update(byte[] key, byte[] content)
        {
            //Redis allows for overwrite
            return Create(key, content);
        }
    }
}
