using System.Threading.Tasks;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;

namespace NeoSharp.Core.Blockchain.Processors
{
    public class EnrollmentTransactionProcessor : IProcessor<EnrollmentTransaction>
    {
        private readonly IRepository _repository;

        public EnrollmentTransactionProcessor(IRepository repository)
        {
            _repository = repository;
        }

        public async Task Process(EnrollmentTransaction stateTx)
        {
            var validator = await _repository.GetValidator(stateTx.PublicKey) ?? new Validator(stateTx.PublicKey);
            validator.Registered = true;
            await _repository.AddValidator(validator);
        }
    }
}