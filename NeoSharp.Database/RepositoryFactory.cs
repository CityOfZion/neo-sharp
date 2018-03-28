using NeoSharp.Database.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoSharp.Database
{
    public abstract class RepositoryFactory
    {
        public abstract IRepository GetRepository(DatabaseProviderType providerType);
    }

    public class DatabaseRepositoryFactory : RepositoryFactory
    {
        public override IRepository GetRepository(DatabaseProviderType providerType)
        {
            switch (providerType)
            {
                case DatabaseProviderType.CosmosDB:
                    return new CosmosDbRepository();
                case DatabaseProviderType.RocksDB:
                    return new RocksDbRepository();   
            }

            //Default to rocks?
            return new RocksDbRepository();
        }
    }
}
