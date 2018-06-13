using NeoSharp.BinarySerialization;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;

namespace NeoSharp.Persistence.RocksDB
{
    public class TransactionRepository : RocksDbRepository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(IRepositoryConfiguration config, IBinarySerializer serializer, IBinaryDeserializer deserializer) 
            : base(config, serializer, deserializer)
        {
        }
    }
}
