using System.Threading.Tasks;
using NeoSharp.Core.Types;
using NeoSharp.Persistence.RedisDB.Helpers;
using StackExchange.Redis;

namespace NeoSharp.Persistence.RedisDB
{
    public interface IRedisDbContext
    {
        Task Set(RedisKey key, RedisValue value);

        Task<RedisValue> Get(RedisKey key);

        Task<bool> AddToIndex(RedisIndex index, UInt256 hash, double indexScore);

        Task<UInt256> GetFromHashIndex(RedisIndex index, double indexScore);
    }
}
