using System.Threading.Tasks;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;

namespace NeoSharp.Core.Blockchain.Processing
{
    public class PublishTransactionPersister : ITransactionPersister<PublishTransaction>
    {
        private readonly IRepository _repository;

        public PublishTransactionPersister(IRepository repository)
        {
            _repository = repository;
        }

        public async Task Persist(PublishTransaction transaction)
        {
            var contract = new Contract
            {
                Code = new Code
                {
                    ScriptHash = transaction.ScriptHash,
                    Script = transaction.Script,
                    ReturnType = transaction.ReturnType,
                    Parameters = transaction.ParameterList
                },
                Name = transaction.Name,
                Version = transaction.CodeVersion,
                Author = transaction.Author,
                Email = transaction.Email,
                Description = transaction.Description
            };

            await _repository.AddContract(contract);
        }
    }
}