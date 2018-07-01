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

            containerBuilder.RegisterSingleton<RocksDbConfig>();
            containerBuilder.RegisterSingleton<IRocksDbRepository, RocksDbRepository>();

            containerBuilder.RegisterSingleton<RedisDbConfig>();
            containerBuilder.RegisterSingleton<IRedisDbRepository, RedisDbRepository>();
        }
    }
}