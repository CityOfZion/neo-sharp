using System.IO;
using Microsoft.Extensions.Configuration;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Persistence
{
    public class PersistenceConfig
    {
        private static PersistenceConfig _persistenceConfig;

        public StorageProvider Provider { get; internal set; }

        public static PersistenceConfig Instance()
        {
            return _persistenceConfig ?? (_persistenceConfig = new PersistenceConfig());
        }

        public PersistenceConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true);
            var configurationRoot = (IConfiguration)builder.Build();

            configurationRoot?
                .GetSection("persistence")?
                .Bind(this);
        }
    }

    public enum StorageProvider
    {
        None,
        RocksDb,
        RedisDbBinary,
        RedisDbJson,
        MongoDb
    }
}
