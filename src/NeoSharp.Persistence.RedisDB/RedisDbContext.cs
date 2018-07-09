using System;
using System.Linq;
using System.Threading.Tasks;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Types;
using NeoSharp.Persistence.RedisDB.Helpers;
using StackExchange.Redis;

namespace NeoSharp.Persistence.RedisDB
{
    public class RedisDbContext : IRedisDbContext
    {
        #region Private Fields 
        private readonly IDatabase _redisDb;
        #endregion

        #region Constructor
        public RedisDbContext(RedisDbConfig config)
        {
            if (config == null)  throw new ArgumentNullException(nameof(config));

            var host = string.IsNullOrEmpty(config.ConnectionString) ? "localhost" : config.ConnectionString;
            var dbId = config.DatabaseId ?? 0;

            var connection = ConnectionMultiplexer.Connect(host);
            this._redisDb = connection.GetDatabase(dbId);

            this.IsBinaryMode = config.IsBinaryMode;
        }
        #endregion

        #region IRedisDbContext implementation 
        public bool IsBinaryMode { get; }

        public Task Set(RedisKey key, RedisValue value)
        {
            return this._redisDb.HashSetAsync(key, new[] { new HashEntry("data", value) });
        }

        public Task<RedisValue> Get(RedisKey key)
        {
            return this._redisDb.HashGetAsync(key, "data");
        }

        public Task<bool> AddToIndex(RedisIndex index, UInt256 hash, double indexScore)
        {
            return this.AddToIndex(index, hash.ToString(), indexScore);
        }

        public async Task<UInt256> GetFromHashIndex(RedisIndex index, double indexScore)
        {
            var values = (await this._redisDb.SortedSetRangeByScoreAsync(index.ToString(), indexScore, indexScore)).ToStringArray();

            return values.Any() ? new UInt256(values.First().HexToBytes()) : null;
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