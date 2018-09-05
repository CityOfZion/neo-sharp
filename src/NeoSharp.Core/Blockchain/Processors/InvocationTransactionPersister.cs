using System;
using System.Threading.Tasks;
using NeoSharp.Core.Models;

namespace NeoSharp.Core.Blockchain.Processors
{
    public class InvocationTransactionPersister: ITransactionPersister<InvocationTransaction>
    {
        public Task Persist(InvocationTransaction invocationTx)
        {
            throw new NotImplementedException();
        }
    }
}