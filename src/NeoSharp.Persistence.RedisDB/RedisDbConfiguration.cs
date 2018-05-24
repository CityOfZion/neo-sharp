using NeoSharp.Core.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoSharp.Persistence.RedisDB
{
    public class RedisDbConfiguration : IRepositoryConfiguration
    {
        public string ConnectionString { get; set; }
        public object DatabaseId { get; set; }
    }
}
