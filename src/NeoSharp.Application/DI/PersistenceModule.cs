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

            var cfg = PersistenceConfig.Instance();

            switch (cfg.Provider)
            {
                case StorageProvider.RedisDbBinary:
                case StorageProvider.RedisDbJson:
                    {
                        containerBuilder.RegisterSingleton<RedisDbConfig>();
                        containerBuilder.RegisterSingleton<IRepository, RedisDbRepository>();
                        break;
                    }
                case StorageProvider.RocksDb:
                    {
                        containerBuilder.RegisterSingleton<RocksDbConfig>();
                        containerBuilder.RegisterSingleton<IRepository, RocksDbRepository>();
                        containerBuilder.RegisterSingleton<IRocksDbContext, RocksDbContext>();
                        break;
                    }
            }
        }
    }
}