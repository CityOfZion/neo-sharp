namespace NeoSharp.Core.Models.OperationManger
{
    public interface ITransactionOperationsManager : ISigner<Transaction>, IVerifier<Transaction>
    {
    }
}
