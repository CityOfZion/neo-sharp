using System.Threading.Tasks;
using NeoSharp.Core.Models;

namespace NeoSharp.Core.Blockchain.Processing
{
    public class InvocationTransactionPersister: ITransactionPersister<InvocationTransaction>
    {
        public Task Persist(InvocationTransaction invocationTx)
        {
            // TODO: Implement it
            return Task.CompletedTask;
        }
    }
}