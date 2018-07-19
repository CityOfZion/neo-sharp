using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Blockchain.Processors
{
    public class IssueTransactionProcessor : IProcessor<IssueTransaction>
    {
        private readonly IRepository _repository;

        public IssueTransactionProcessor(IRepository repository)
        {
            _repository = repository;
        }

        public async Task Process(IssueTransaction issueTx)
        {
            var inputsTasks = issueTx.Inputs.Select(async coin =>
                (await _repository.GetTransaction(coin.PrevHash)).Outputs[coin.PrevIndex]);
            var inputs = await Task.WhenAll(inputsTasks);
            var assetChanges = new Dictionary<UInt256, Fixed8>();
            foreach (var input in inputs)
            {
                if (assetChanges.ContainsKey(input.AssetId))
                    assetChanges[input.AssetId] -= input.Value;
                else
                    assetChanges[input.AssetId] = -input.Value;
            }

            foreach (var output in issueTx.Outputs)
            {
                if (assetChanges.ContainsKey(output.AssetId))
                    assetChanges[output.AssetId] += output.Value;
                else
                    assetChanges[output.AssetId] = output.Value;
            }

            foreach (var keypair in assetChanges)
            {
                if (keypair.Value == Fixed8.Zero) continue;
                var asset = await _repository.GetAsset(keypair.Key);
                asset.Available += keypair.Value;
                await _repository.AddAsset(asset);
            }
        }
    }
}