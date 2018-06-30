using NeoSharp.Core.DI;
using NeoSharp.Core.Persistence;
using NeoSharp.Persistence.RocksDB;
using NeoSharp.Persistence.RedisDB;

namespace NeoSharp.Application.DI
{
    public class PersistenceModule : IModule
    {
        public void Register(IContainerBuilder containerBuilder)
        {
            //TODO:  Instantiate the Binary or JSON Persistence providers based on configuration (will need to implement provider types in configuration files)
            //Examples:

            ////Binary storage provider config
            //if (BinaryStorageProvider == RocksDb)
            //{
            //    var config = new RocksDbConfiguration
            //    {
            //        ConnectionString = "localhost",
            //        StorageFormat = RepositoryPersistenceFormat.Binary
            //    };

            //    new RocksDbRepository();
            //}
            //else if (BinaryStorageProvider == RedisDb)
            //{
            //    var config = new RedisDbConfiguration
            //    {
            //        ConnectionString = "localhost",
            //        DatabaseId = 0,
            //        StorageFormat = RepositoryPersistenceFormat.Binary
            //    };

            //    new RedisDbRepository();
            //}

            ////Json storage provider config
            //if (JsonStorageProvider == RedisDB)
            //{
            //    var config = new RedisDbConfiguration
            //    {
            //        ConnectionString = "localhost",
            //        DatabaseId = 1, //If were going to use redis for both, we can change the id
            //        StorageFormat = RepositoryPersistenceFormat.JSON
            //    };

            //    new RedisDbRepository();
            //}
            //else if (JsonStorageProvider == MongoDB)
        }
    }
}