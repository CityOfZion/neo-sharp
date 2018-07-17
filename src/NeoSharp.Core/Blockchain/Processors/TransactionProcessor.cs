using System.Linq;
using System.Threading.Tasks;
using NeoSharp.Core.Blockchain.State;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Blockchain.Processors
{
    /// <summary>
    ///     Processes the common properties of a Transaction.
    ///     Processes the outputs by creating the coin states for them and updating balances.
    ///     Processes the inputs by marking their state with CoinState.Spent and updating balances.
    ///     Calls the respective transaction type processors for further processing.
    /// </summary>
    public class TransactionProcessor : IProcessor<Transaction>
    {
        private readonly IRepository _repository;
        private readonly IProcessor<ClaimTransaction> _claimTxProcessor;
        private readonly IProcessor<InvocationTransaction> _invocationTxProcessor;
        private readonly IAccountManager _accountManager;

        public TransactionProcessor(
            IRepository repository,
            IAccountManager accountManager,
            IProcessor<ClaimTransaction> claimTxProcessor,
            IProcessor<InvocationTransaction> invocationTxProcessor
        )
        {
            _repository = repository;
            _accountManager = accountManager;
            _claimTxProcessor = claimTxProcessor;
            _invocationTxProcessor = invocationTxProcessor;
        }

        public async Task Process(Transaction tx)
        {
            await GainOutputs(tx.Hash, tx.Outputs);
            await SpendOutputs(tx.Inputs);

            switch (tx)
            {
                case ContractTransaction _:
                case MinerTransaction _:
                    break;
                case ClaimTransaction claimTx:
                    await _claimTxProcessor.Process(claimTx);
                    break;
                case InvocationTransaction invocationTx:
                    await _invocationTxProcessor.Process(invocationTx);
                    break;
                //TODO: Resume throwing once every type is implemented
//                default:
//                    throw new ArgumentException("Unknown Transaction Type");
            }

            await _repository.AddTransaction(tx);
        }

        private async Task GainOutputs(UInt256 hash, TransactionOutput[] outputs)
        {
            foreach (var output in outputs)
                await _accountManager.UpdateBalance(output.ScriptHash, output.AssetId, output.Value);

            var newCoinStates = outputs.Select(o => CoinState.New).ToArray();
            await _repository.AddCoinStates(hash, newCoinStates);
        }

        private async Task SpendOutputs(CoinReference[] inputs)
        {
            foreach (var inputGroup in inputs.GroupBy(i => i.PrevHash))
            {
                var prevTxHash = inputGroup.Key;
                var tx = await _repository.GetTransaction(prevTxHash);
                var coinStates = await _repository.GetCoinStates(prevTxHash);
                foreach (var coinReference in inputGroup)
                {
                    coinStates[coinReference.PrevIndex] |= CoinState.Spent;

                    var output = tx.Outputs[coinReference.PrevIndex];
                    await _accountManager.UpdateBalance(output.ScriptHash, output.AssetId, -output.Value);
                }

                await _repository.AddCoinStates(prevTxHash, coinStates);
            }
        }
    }
}