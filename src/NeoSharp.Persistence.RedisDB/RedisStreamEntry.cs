using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoSharp.Persistence.RedisDB
{
    public class RedisStreamEntry
    {
        public RedisResult Id { get; set; }
        public RedisResult Key { get; set; }
        public RedisResult Value { get; set; }
    }
}
