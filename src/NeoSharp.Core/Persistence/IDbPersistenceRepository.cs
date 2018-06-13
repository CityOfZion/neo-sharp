namespace NeoSharp.Core.Persistence
{
    public interface IDbPersistenceRepository
    {
        IBlockHeaderRepository BlockHeaderRepository { get; }

        ITransactionRepository TransactionRepository { get; }
    }
}
