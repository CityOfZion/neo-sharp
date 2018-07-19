using System.Threading.Tasks;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;

namespace NeoSharp.Core.Blockchain.Processors
{
    public class PublishTransactionProcessor : IProcessor<PublishTransaction>
    {
        private readonly IRepository _repository;

        public PublishTransactionProcessor(IRepository repository)
        {
            _repository = repository;
        }

        public async Task Process(PublishTransaction publishTx)
        {
            var contract = new Contract
            {
                Code = new Code
                {
                    ScriptHash = publishTx.ScriptHash,
                    Script = publishTx.Script,
                    ReturnType = publishTx.ReturnType,
                    Parameters = publishTx.ParameterList
                },
                Name = publishTx.Name,
                Version = publishTx.CodeVersion,
                Author = publishTx.Author,
                Email = publishTx.Email,
                Description = publishTx.Description
            };
            await _repository.AddContract(contract);
        }
    }
}