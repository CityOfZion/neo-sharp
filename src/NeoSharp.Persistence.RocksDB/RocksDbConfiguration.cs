using System;
using NeoSharp.Core.Persistence;

namespace NeoSharp.Persistence.RocksDB
{
    public class RocksDbConfiguration : IRepositoryConfiguration
    {
        public string ConnectionString { get; set; }

        public object DatabaseId { get; set; }

        public RepositoryPersistenceFormat StorageFormat { get; set; }
    }
}
