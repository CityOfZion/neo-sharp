namespace NeoSharp.Core.Models.OperationManger
{
    public interface ITransactionOperationsManager
    {
        void Sign(Transaction transaction);

        bool Verify(Transaction transaction);
    }
}
