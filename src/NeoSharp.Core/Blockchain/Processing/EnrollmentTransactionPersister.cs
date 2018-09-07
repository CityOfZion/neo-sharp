using System.Threading.Tasks;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;

namespace NeoSharp.Core.Blockchain.Processing
{
    public class EnrollmentTransactionPersister : ITransactionPersister<EnrollmentTransaction>
    {
        private readonly IRepository _repository;

        public EnrollmentTransactionPersister(IRepository repository)
        {
            _repository = repository;
        }

        public async Task Persist(EnrollmentTransaction transaction)
        {
            var validator = await _repository.GetValidator(transaction.PublicKey) ?? new Validator(transaction.PublicKey);

            validator.Registered = true;

            await _repository.AddValidator(validator);
        }
    }
}