using NeoSharp.Core.Extensions;
using NeoSharp.Core.Persistence;
using StackExchange.Redis;

namespace NeoSharp.Persistence.RedisDB
{
    public class RedisDatabaseHelper
    {
        private readonly IDatabase _redisDb;

        public RedisDatabaseHelper(IDatabase redisDb)
        {
            _redisDb = redisDb;
        }

        /// <summary>
        /// Sets the values in a hash set at key.  Using a Redis HashSet will give us O(1) complexity on search
        /// </summary>
        /// <param name="type">The data entry type</param>
        /// <param name="key">The key to use</param>
        /// <param name="value">The value to be written as the hash entry</param>
        public void Set(DataEntryPrefix type, string key, RedisValue value)
        {
            _redisDb.HashSet(BuildKey(type, key), new HashEntry[] { new HashEntry("data", value) });
        }

        /// <summary>
        /// Gets the values from a hash set at key.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public RedisValue Get(DataEntryPrefix type, string key)
        {
            return _redisDb.HashGet(BuildKey(type, key), "data");
        }

        /// <summary>
        /// Gets the values from a hash set at key.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public RedisValue Get(DataEntryPrefix type, byte[] key)
        {
            return Get(type, key.ToHexString());
        }

        /// <summary>
        /// Adds a specified value and score to a Redis Sorted Set.  Using a Redis Sorted Set will give us a O(log(n)) complexity on search.
        /// </summary>
        /// <param name="index">Index to write</param>
        /// <param name="indexScore">Score to assign</param>
        /// <param name="value">Value associated with score</param>
        public void AddToIndex(RedisIndex index, double indexScore, string value)
        {
            _redisDb.SortedSetAdd(index.ToString(), value, indexScore);
        }

        /// <summary>
        /// Retrieves a specified value from a Redis Sorted Set based on score.
        /// </summary>
        /// <param name="index">Index to search</param>
        /// <param name="indexScore">Score to retrieve.</param>
        /// <returns></returns>
        public string[] GetFromIndex(RedisIndex index, double indexScore)
        {
            return _redisDb.SortedSetRangeByScore(index.ToString(), indexScore, indexScore).ToStringArray();
        }


        /// <summary>
        /// Retrieves a specified range of values from a Redis Sorted Set based on a range of scores
        /// </summary>
        /// <param name="index">Index to search</param>
        /// <param name="startIndexScore">Range starting score</param>
        /// <param name="endIndexScore">Range end score</param>
        /// <returns></returns>
        public string[] GetRangeFromIndex(RedisIndex index, double startIndexScore, double endIndexScore)
        {
            return _redisDb.SortedSetRangeByScore(index.ToString(), startIndexScore, endIndexScore).ToStringArray();
        }

        /// <summary>
        /// Retrieves the length / number of elements in an index
        /// </summary>
        /// <param name="index">Index to retrieve the length of</param>
        /// <returns></returns>
        public long GetIndexLength(RedisIndex index)
        {
            return _redisDb.SortedSetLength(index.ToString());
        }

        /// <summary>
        /// Builds the concatenated key based on data type and desired key
        /// </summary>
        /// <param name="type">Data type</param>
        /// <param name="key">Desired key</param>
        /// <returns></returns>
        private string BuildKey(DataEntryPrefix type, string key)
        {
            return string.Format("{0}:{1}", key, type.ToString());
        }
    }
}
