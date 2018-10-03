using System;
using System.Linq;
using System.Threading.Tasks;
using NeoSharp.Persistence.RedisDB.Helpers;
using NeoSharp.Types;
using NeoSharp.Types.ExtensionMethods;
using StackExchange.Redis;

namespace NeoSharp.Persistence.RedisDB
{
    public class RedisDbContext : IRedisDbContext
    {
        #region Private Fields 

        private readonly IDatabase _redisDb;
        private readonly ConnectionMultiplexer _connection;

        #endregion

        #region Constructor

        public RedisDbContext(RedisDbBinaryConfig binaryConfig)
        {
            if (binaryConfig == null)  throw new ArgumentNullException(nameof(binaryConfig));

            var host = string.IsNullOrEmpty(binaryConfig.ConnectionString) ? "localhost" : binaryConfig.ConnectionString;
            var dbId = binaryConfig.DatabaseId ?? 0;

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

        public Task<bool> AddToIndex(RedisIndex index, UInt256 hash, double indexScore)
        {
            return this.AddToIndex(index, hash.ToString(), indexScore);
        }

        public async Task<UInt256> GetFromHashIndex(RedisIndex index, double indexScore)
        {
            var values = (await _redisDb.SortedSetRangeByScoreAsync(index.ToString(), indexScore, indexScore)).ToStringArray();

            return values.Any() ? new UInt256(values.First().HexToBytes()) : null;
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