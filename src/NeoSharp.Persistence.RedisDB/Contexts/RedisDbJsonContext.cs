using NeoSharp.Core.Persistence;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NeoSharp.Persistence.RedisDB
{
    public class RedisDbJsonContext : IDbJsonContext
    {
        private readonly IRepositoryConfiguration _config;
        private readonly IDatabase _redisDb;

        public RedisDbJsonContext(IRepositoryConfiguration config)
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

        public Task Create(string key, string content)
        {
            var taskCompletionSource = new TaskCompletionSource<object>();

            Task.Factory.StartNew(() =>
            {
                _redisDb.HashSet(key, new HashEntry[] { new HashEntry("data", content) });
                taskCompletionSource.SetResult(null);
            });

            return taskCompletionSource.Task;
        }

        public Task Delete(string key)
        {
            throw new NotImplementedException();
        }

        public Task<string> Get(string key)
        {
            var taskCompletionSource = new TaskCompletionSource<string>();

            Task.Factory.StartNew(() =>
            {
                var rawContent = _redisDb.HashGet(key, "data");
                taskCompletionSource.SetResult(rawContent);
            });

            return taskCompletionSource.Task;
        }

        public Task Update(string key, string content)
        {
            //Redis allows for overwrite
            return Create(key, content);
        }
    }
}
