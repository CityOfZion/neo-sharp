using System.IO;
using Microsoft.Extensions.Configuration;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Persistence
{
    public class PersistenceConfig
    {
        private static PersistenceConfig _persistenceConfig;

        public BinaryStorageProvider BinaryStorageProvider { get; internal set; }

        public JsonStorageProvider JsonStorageProvider { get; internal set; }

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

    public enum BinaryStorageProvider
    {
        None,
        RocksDb,
        RedisDb
    }

    public enum JsonStorageProvider
    {
        None,
        RedisDb,
        MongoDb
    }
}
