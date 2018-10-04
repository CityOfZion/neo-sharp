using NeoSharp.Persistence.RedisDB.Helpers;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeoSharp.Types;

namespace NeoSharp.Persistence.RedisDB
{
    public class RedisDbJsonContext : IRedisDbJsonContext
    {
        #region Private Fields 

        private readonly IDatabase _redisDb;
        private readonly ConnectionMultiplexer _connection;

        #endregion

        #region Constructor

        public RedisDbJsonContext(RedisDbJsonConfig jsonConfig)
        {
            if (jsonConfig == null) throw new ArgumentNullException(nameof(jsonConfig));

            var host = string.IsNullOrEmpty(jsonConfig.ConnectionString) ? "localhost" : jsonConfig.ConnectionString;
            var dbId = jsonConfig.DatabaseId ?? 0;

            _connection = ConnectionMultiplexer.Connect(host);
            _redisDb = _connection.GetDatabase(dbId);
        }

        #endregion

        #region IRedisDbContext implementation

        public Task Set(RedisKey key, RedisValue value)
        {
            return _redisDb.HashSetAsync(key, new[] { new HashEntry("data", value) });
        }

        public Task<RedisValue> Get(RedisKey key)
        {
            return _redisDb.HashGetAsync(key, "data");
        }

        public async Task<Dictionary<RedisKey, RedisValue>> GetMany(RedisKey[] keys)
        {
            var keyValueTasks = keys.ToDictionary(k => k, Get);

            await Task.WhenAll(keyValueTasks.Values);

            return keyValueTasks.ToDictionary(kv => kv.Key, kv => kv.Value.Result);
        }

        public Task<bool> AddToIndex(RedisIndex index, UInt256 hash, double indexScore)
        {
            return this.AddToIndex(index, hash.ToString(), indexScore);
        }

        public async Task<UInt256> GetFromHashIndex(RedisIndex index, double indexScore)
        {
            var values = (await _redisDb.SortedSetRangeByScoreAsync(index.ToString(), indexScore, indexScore)).ToStringArray();

            return values.Any() ? UInt256.Parse(values.First()) : null;
        }

        public async Task<bool> Delete(RedisKey key)
        {
            return await _redisDb.KeyDeleteAsync(key);
        }

        public void Dispose()
        {
            _connection?.Close();
        }

        #endregion

        #region Private Methods

        private Task<bool> AddToIndex(RedisIndex index, string value, double indexScore)
        {
            return this._redisDb.SortedSetAddAsync(index.ToString(), value, indexScore);
        }

        #endregion
    }
}
