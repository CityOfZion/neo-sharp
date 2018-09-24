namespace NeoSharp.Core.Models.OperationManger
{
    public interface ITransactionSigner
    {
        void Sign(Transaction transaction);
    }
}
