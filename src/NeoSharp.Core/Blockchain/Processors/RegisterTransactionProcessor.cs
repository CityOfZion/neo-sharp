using System.Threading.Tasks;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Blockchain.Processors
{
    public class RegisterTransactionProcessor : IProcessor<RegisterTransaction>
    {
        private readonly IRepository _repository;

        public RegisterTransactionProcessor(IRepository repository)
        {
            _repository = repository;
        }

        public async Task Process(RegisterTransaction registerTx)
        {
            await _repository.AddAsset(new Asset
            {
                Id = registerTx.Hash,
                AssetType = registerTx.AssetType,
                Name = registerTx.Name,
                Amount = registerTx.Amount,
                Available = Fixed8.Zero,
                Precision = registerTx.Precision,
                Owner = registerTx.Owner,
                Admin = registerTx.Admin
            });
        }
    }
}