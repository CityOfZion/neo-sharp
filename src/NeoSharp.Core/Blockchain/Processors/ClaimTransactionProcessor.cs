using System.Linq;
using System.Threading.Tasks;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;

namespace NeoSharp.Core.Blockchain.Processors
{
    /// <summary>
    /// Special processing for ClaimTransactions.
    /// The coin states that are claimed are marked as CoinState.Claimed.
    /// The increment of GAS of the account is done by the TransactionProcessor.
    /// </summary>
    public class ClaimTransactionProcessor: IProcessor<ClaimTransaction>
    {
        private IRepository _repository;

        public ClaimTransactionProcessor(IRepository repository)
        {
            _repository = repository;
        }

        public async Task Process(ClaimTransaction claimTx)
        {
            foreach (var txGroup in claimTx.Claims.GroupBy(c => c.PrevHash))
            {
                var coinStates = await _repository.GetCoinStates(txGroup.Key);
                foreach (var reference in txGroup) coinStates[reference.PrevIndex] |= CoinState.Claimed;
                await _repository.AddCoinStates(txGroup.Key, coinStates);
            }
        }
    }
}