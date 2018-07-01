using StackExchange.Redis;

namespace NeoSharp.Persistence.RedisDB.Helpers
{
    /// <summary>
    /// Helper class to encapsulate all redis Database and Streams setup and behaviors
    /// </summary>
    public class RedisHelper
    {
        public RedisDatabaseHelper Database { get; set; }
        public RedisStreamsHelper Streams { get; set; }

        public RedisHelper(string connectionString, int? dbNum)
        {
            var connection = ConnectionMultiplexer.Connect(connectionString);

            //Retrieve the Redis DB with the specified number (by default, there are 16), default to 0.
            var db = connection.GetDatabase(dbNum.HasValue ? dbNum.Value : 0);

            //Initialize helper / wrappers with the database.
            Database = new RedisDatabaseHelper(db);
            Streams = new RedisStreamsHelper(db);
        }
    }
}
