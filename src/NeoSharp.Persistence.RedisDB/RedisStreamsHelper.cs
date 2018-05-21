using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NeoSharp.Persistence.RedisDB
{
    /// <summary>
    /// Wrapper to encapsulate all Redis Streams API implementation
    /// </summary>
    public class RedisStreamsHelper
    {
        private readonly IDatabase _redisDb;

        public RedisStreamsHelper(IDatabase redisDb)
        {
            _redisDb = redisDb;
        }

        /// <summary>
        /// Returns the number of elements in the specified stream
        /// </summary>
        /// <param name="streamName">Name of the stream</param>
        /// <returns>Number of elements in the specified stream</returns>
        public RedisResult XLen(RedisValue streamName)
        {
            return _redisDb.Execute("XLEN", streamName);
        }

        /// <summary>
        /// Appends an element to the specified stream
        /// </summary>
        /// <param name="streamName">Name of the stream</param>
        /// <param name="id">Explicit identifier and instance number for the entry in the stream (ie:1523598632996-0) </param>
        /// <param name="key">The key for the entry</param>
        /// <param name="value">The value for the entry</param>
        /// <returns>The identifier of the element that was appended to the stream</returns>
        public RedisResult XAdd(RedisValue streamName, RedisValue? id, RedisValue key, RedisValue value)
        {
            if (!id.HasValue)
                id = "*"; //Auto assign id

            Debug.Assert(id != null, nameof(id) + " != null");

            return _redisDb.Execute("XADD", streamName, id.Value, key, value);
        }

        /// <summary>
        /// Retrieves a range of elements from the specified stream
        /// </summary>
        /// <param name="streamName">Name of the stream</param>
        /// <param name="start">The identifier or timestamp in ms of the range start</param>
        /// <param name="end">The identifier or timestamp in ms of the range end</param>
        /// <returns></returns>
        public List<RedisStreamEntry> XRange(RedisValue streamName, RedisValue? start, RedisValue? end)
        {
            if (!start.HasValue && !end.HasValue)
            {
                //Return all results
                start = "-";
                end = "+";
            }
            //If only a single value is provided, assume they are targeting a single record
            else if (start.HasValue && !end.HasValue)
                end = start;
            else if (!start.HasValue)
                start = end;

            List<RedisStreamEntry> entries = new List<RedisStreamEntry>();
            var res = _redisDb.Execute("XRANGE", streamName, start, end);
            foreach (RedisResult[] item in (RedisResult[])res)
            {
                var keyValue = (RedisResult[])item[1];
                entries.Add(new RedisStreamEntry
                {
                    Id = item[0],
                    Key = keyValue[0],
                    Value = keyValue[1]
                });
            }

            return entries;
        }
    }
}
