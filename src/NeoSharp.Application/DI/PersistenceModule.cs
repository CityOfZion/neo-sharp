using NeoSharp.Core.DI;
using NeoSharp.Core.Persistence;
using NeoSharp.Persistence.RedisDB;
using NeoSharp.Persistence.RocksDB;

namespace NeoSharp.Application.DI
{
    public class PersistenceModule : IModule
    {
        public void Register(IContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterSingleton<PersistenceConfig>();
            containerBuilder.RegisterSingleton<IRepository, NeoSharpRepository>();

            if (PersistenceConfig.Instance().BinaryStorageProvider == BinaryStorageProvider.RocksDb)
            {
                // register RocksDbBinaryRepository
                containerBuilder.RegisterSingleton<RocksDbConfig>();
                containerBuilder.RegisterSingleton<IRocksDbRepository, RocksDbRepository>();
            }

            if (PersistenceConfig.Instance().BinaryStorageProvider == BinaryStorageProvider.RedisDb)
            {
                // register redisDbBinaryRepository
                containerBuilder.RegisterSingleton<RedisDbConfig>();
                containerBuilder.RegisterSingleton<IRedisDbRepository, RedisDbRepository>();
            }

            if (PersistenceConfig.Instance().JsonStorageProvider == JsonStorageProvider.RedisDb)
            {
                // register RedisDbJsonRepository
                containerBuilder.RegisterSingleton<RedisDbRepository>();
            }

            if (PersistenceConfig.Instance().JsonStorageProvider == JsonStorageProvider.MongoDb)
            {
                // register MongoDbJsonRepository
            }
        }
    }
}