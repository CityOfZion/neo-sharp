using NeoSharp.Core.DI;
using NeoSharp.Core.Persistence;
using NeoSharp.Persistence.RocksDB;

namespace NeoSharp.Application.DI
{
    public class PersistenceModule : IModule
    {
        public void Register(IContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterSingleton<IRepositoryConfiguration, RocksDbConfiguration>();

            // TODO: if RocksDb is configured, register this objects for DI
            containerBuilder.RegisterSingleton<IDbPersistenceRepository, RocksDbPersistenceRepository>();
            containerBuilder.RegisterSingleton<IBlockHeaderRepository, BlockHeaderRepository>();
            containerBuilder.RegisterSingleton<ITransactionRepository, TransactionRepository>();

            // TODO: else, if RedisDb is configured, register this objets for DI

            // TODO: else, any other database implementation
        }
    }
}