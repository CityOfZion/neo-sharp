using System.Threading.Tasks;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.Types;

namespace NeoSharp.Core.Blockchain.Processing
{
    public class RegisterTransactionPersister : ITransactionPersister<RegisterTransaction>
    {
        private readonly IRepository _repository;

        public RegisterTransactionPersister(IRepository repository)
        {
            _repository = repository;
        }

        public async Task Persist(RegisterTransaction transaction)
        {
            await _repository.AddAsset(new Asset
            {
                Id = transaction.Hash,
                AssetType = transaction.AssetType,
                Name = transaction.Name,
                Amount = transaction.Amount,
                Available = Fixed8.Zero,
                Precision = transaction.Precision,
                Owner = transaction.Owner,
                Admin = transaction.Admin
            });
        }
    }
}