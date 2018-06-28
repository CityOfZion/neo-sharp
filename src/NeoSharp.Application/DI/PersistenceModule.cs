using NeoSharp.Core.DI;
using NeoSharp.Core.Persistence;
using NeoSharp.Core.Persistence.Contexts;
using NeoSharp.Persistence.RocksDB;

namespace NeoSharp.Application.DI
{
    public class PersistenceModule : IModule
    {
        public void Register(IContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterSingleton<IRepositoryConfiguration, RocksDbConfiguration>();

            containerBuilder.RegisterSingleton<IBlockHeaderContext, BlockHeaderContext>();
            containerBuilder.RegisterSingleton<ITransactionContext, TransactionContext>();

            // TODO: if RocksDb is configured, register this objects for DI
            containerBuilder.RegisterSingleton<IDbModel, RocksDbModel>();
            containerBuilder.RegisterSingleton<IDbContext, RocksDbContext>();

            // TODO: else, if RedisDb is configured, register this objets for DI

            // TODO: else, any other database implementation
        }
    }
}