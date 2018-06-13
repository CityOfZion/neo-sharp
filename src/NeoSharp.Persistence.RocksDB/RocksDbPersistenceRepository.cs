using NeoSharp.Core.Persistence;

namespace NeoSharp.Persistence.RocksDB
{
    public class RocksDbPersistenceRepository : IDbPersistenceRepository
    {
        public IBlockHeaderRepository BlockHeaderRepository { get; private set; }

        public ITransactionRepository TransactionRepository { get; private set; }

        public RocksDbPersistenceRepository(
            IBlockHeaderRepository blockHeaderRepository,
            ITransactionRepository transactionRepository)
        {
            this.BlockHeaderRepository = blockHeaderRepository;
            this.TransactionRepository = transactionRepository;
        }
    }
}
