using NeoSharp.Core.DI;
using NeoSharp.Core.Persistence;
using NeoSharp.Core.Persistence.Contexts;
using NeoSharp.Persistence.RocksDB;
using NeoSharp.Persistence.RedisDB;

namespace NeoSharp.Application.DI
{
    public class PersistenceModule : IModule
    {
        public void Register(IContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterSingleton<IRepositoryConfiguration, RocksDbConfiguration>();
            //TODO: Register if configured to use Redis for either binary or json persistence
            //containerBuilder.RegisterSingleton<IRepositoryConfiguration, RedisDbConfiguration>();

            containerBuilder.RegisterSingleton<IBlockHeaderContext, BlockHeaderContext>();
            containerBuilder.RegisterSingleton<ITransactionContext, TransactionContext>();

            //TODO: Register our persistence types based on the configuration for binary and JSON
            //TODO: These should not be singleton
            //if (BinaryPersistenceContect = Rocks)
            //{
                containerBuilder.RegisterSingleton<IDbModel, RocksDbBinaryModel>();
                containerBuilder.RegisterSingleton<IDbBinaryContext, RocksDbBinaryContext>();
            //}
            //else if (BinaryPersistenceContext == Redis)
            //{
            //    ConfigurationModule.RedisConnectionString->Redis Server B
            //    containerBuilder.RegisterSingleton<IDbBinaryModel, RedisBinaryDbModel>();
            //    containerBuilder.RegisterSingleton<IDbBinaryContext, RedisBinaryDbContext>();
            //}

            //if (JsonPersistenceContext = Redis)
            //{
            //    containerBuilder.RegisterSingleton<IDbBinaryModel, RedisJsonDbModel>();
            //    containerBuilder.RegisterSingleton<IDbJsonContext, RedisJsonDbContext>();
            //}
        }
    }
}