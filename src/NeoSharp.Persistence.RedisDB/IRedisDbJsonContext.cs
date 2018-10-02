using NeoSharp.Core.Types;
using NeoSharp.Persistence.RedisDB.Helpers;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
