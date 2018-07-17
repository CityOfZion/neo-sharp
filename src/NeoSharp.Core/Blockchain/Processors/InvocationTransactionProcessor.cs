using System;
using System.Threading.Tasks;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;

namespace NeoSharp.Core.Blockchain.Processors
{
    public class InvocationTransactionProcessor: IProcessor<InvocationTransaction>
    {
        private IRepository _repository;

        public InvocationTransactionProcessor(IRepository repository)
        {
            _repository = repository;
        }

        public async Task Process(InvocationTransaction invocationTx)
        {
            throw new NotImplementedException();
        }
    }
}