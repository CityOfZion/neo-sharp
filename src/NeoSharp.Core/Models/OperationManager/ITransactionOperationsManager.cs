using System.Threading.Tasks;
using NeoSharp.Types;

namespace NeoSharp.Core.Models.OperationManager
{
    public interface ITransactionOperationsManager : ISigner<Transaction>, IVerifier<Transaction>
    {
        Task<UInt160[]> GetScriptHashes(Transaction transaction);
    }
}
