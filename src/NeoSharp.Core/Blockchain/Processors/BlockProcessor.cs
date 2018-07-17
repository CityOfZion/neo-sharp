using System.Threading.Tasks;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;

namespace NeoSharp.Core.Blockchain.Processors
{
    public class BlockProcessor : IProcessor<Block>
    {
        private readonly IRepository _repository;
        private readonly IProcessor<Transaction> _transactionProcessor;

        public BlockProcessor(IRepository repository, IProcessor<Transaction> transactionProcessor)
        {
            _repository = repository;
            _transactionProcessor = transactionProcessor;
        }

        public async Task Process(Block item)
        {
            foreach (var tx in item.Transactions)
                await _transactionProcessor.Process(tx);

            await _repository.AddBlockHeader(item.GetBlockHeader());
            await _repository.SetTotalBlockHeight(item.Index);
        }
    }
}