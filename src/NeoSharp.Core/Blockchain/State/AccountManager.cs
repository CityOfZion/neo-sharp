using System.Threading.Tasks;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Blockchain.State
{
    public class AccountManager : IAccountManager
    {
        private readonly IRepository _repository;

        public AccountManager(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<Account> Get(UInt160 hash)
        {
            return await _repository.GetAccount(hash);
        }

        public async Task UpdateBalance(UInt160 hash, UInt256 assetId, Fixed8 delta)
        {
            var account = await Get(hash);
            if (account.Balances.ContainsKey(assetId))
                account.Balances[assetId] += delta;
            else
                account.Balances[assetId] = delta;

            await _repository.AddAccount(account);

            if (assetId.Equals(GenesisAssets.GoverningTokenRegisterTransaction.Hash) && account.Votes.Length > 0)
                foreach (var pubKey in account.Votes)
                {
                    var validator = await _repository.GetValidator(pubKey);
                    validator.Votes += delta;
                    await _repository.AddValidator(validator);
                }

            // TODO: Check if we need to store validatorCount because existing implementation has it
//                validators_count.GetAndChange().Votes[account.Votes.Length - 1] += output.Value;
        }
    }
}