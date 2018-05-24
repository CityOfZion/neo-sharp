using NeoSharp.Core.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoSharp.Persistence.RocksDB
{
    public class RocksDbConfiguration : IRepositoryConfiguration
    {
        public string ConnectionString { get; set; }
        public object DatabaseId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
