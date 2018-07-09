using System;
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
                case RedisDbConfig.Provider:
                    {
                        containerBuilder.RegisterSingleton<RedisDbConfig>();
                        containerBuilder.RegisterSingleton<IRepository, RedisDbRepository>();
                        containerBuilder.RegisterSingleton<IRedisDbContext, RedisDbContext>();
                        break;
                    }

                case RocksDbConfig.Provider:
                    {
                        containerBuilder.RegisterSingleton<RocksDbConfig>();
                        containerBuilder.RegisterSingleton<IRepository, RocksDbRepository>();
                        containerBuilder.RegisterSingleton<IRocksDbContext, RocksDbContext>();
                        break;
                    }

                default:
                    throw new Exception($"The persistence configuration contains unknown provider \"{cfg.Provider}\"");
            }
        }
    }
}