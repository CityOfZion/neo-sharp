using System;
using System.Threading.Tasks;
using NeoSharp.Persistence.RedisDB.Helpers;
using NeoSharp.Types;
using StackExchange.Redis;

namespace NeoSharp.Persistence.RedisDB
{
    public interface IRedisDbContext : IDisposable
    {
        Task Set(RedisKey key, RedisValue value);

        Task<RedisValue> Get(RedisKey key);

        Task<bool> AddToIndex(RedisIndex index, UInt256 hash, double indexScore);

        Task<UInt256> GetFromHashIndex(RedisIndex index, double indexScore);

        Task<bool> Delete(RedisKey key);
    }
}
