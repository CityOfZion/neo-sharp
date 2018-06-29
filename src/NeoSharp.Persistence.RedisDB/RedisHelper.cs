using NeoSharp.Core.Persistence;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Diagnostics;

namespace NeoSharp.Persistence.RedisDB
{
    ///// <summary>
    ///// Helper class to encapsulate all redis Database and Streams setup and behaviors
    ///// </summary>
    //public class RedisHelper
    //{
    //    public RedisDatabaseHelper Database { get; set; }
    //    public RedisStreamsHelper Streams { get; set; }

    //    public RedisHelper(ConnectionMultiplexer redis, int? dbNum)
    //    {
    //        //Retrieve the Redis DB with the specified number (by default, there are 16), default to 0.
    //        var db = redis.GetDatabase(dbNum.HasValue ? dbNum.Value : 0);

    //        //Initialize helper / wrappers with the database.
    //        Database = new RedisDatabaseHelper(db);
    //        Streams = new RedisStreamsHelper(db);
    //    }
    //}
}
