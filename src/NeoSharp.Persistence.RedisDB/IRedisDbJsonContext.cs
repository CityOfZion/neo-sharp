using NeoSharp.Persistence.RedisDB.Helpers;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;
using NeoSharp.Types;

namespace NeoSharp.Persistence.RedisDB
{
    public interface IRedisDbJsonContext : IDisposable
    {
        Task Set(RedisKey key, RedisValue value);

        Task<RedisValue> Get(RedisKey key);

        Task<bool> AddToIndex(RedisIndex index, UInt256 hash, double indexScore);

        Task<UInt256> GetFromHashIndex(RedisIndex index, double indexScore);

        Task<bool> Delete(RedisKey key);
    }
}
