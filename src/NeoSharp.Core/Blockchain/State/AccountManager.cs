using System.Threading.Tasks;
using NeoSharp.Core.Blockchain.Genesis;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.Types;

namespace NeoSharp.Core.Blockchain.State
{
    public class AccountManager : IAccountManager
    {
        private readonly IRepository _repository;
        private readonly IGenesisAssetsBuilder _genesisAssets;

        public AccountManager(IRepository repository, IGenesisAssetsBuilder genesisAssets)
        {
            _repository = repository;
            _genesisAssets = genesisAssets;
        }

        public async Task<Account> Get(UInt160 hash)
        {
            return await _repository.GetAccount(hash);
        }

        public async Task UpdateBalance(UInt160 hash, UInt256 assetId, Fixed8 delta)
        {
            var account = await Get(hash) ?? new Account(hash);
            if (account.Balances.ContainsKey(assetId))
                account.Balances[assetId] += delta;
            else
                account.Balances[assetId] = delta;

            await _repository.AddAccount(account);

            if (assetId.Equals(_genesisAssets.BuildGoverningTokenRegisterTransaction().Hash) && account.Votes?.Length > 0)
                foreach (var pubKey in account.Votes)
                    await UpdateValidatorVote(pubKey, delta);

            // TODO #382: Check if we need to store validatorCount because existing implementation has it
//                validators_count.GetAndChange().Votes[account.Votes.Length - 1] += output.Value;
        }

        public async Task UpdateVotes(UInt160 hash, ECPoint[] newCandidates)
        {
            var account = await Get(hash) ?? new Account(hash);
            var governingTokenBalance = account.Balances[_genesisAssets.BuildGoverningTokenRegisterTransaction().Hash];

            foreach (var keyOfOldValidator in account.Votes)
                await UpdateValidatorVote(keyOfOldValidator, -governingTokenBalance);

            foreach (var keyOfNewValidator in newCandidates)
                await UpdateValidatorVote(keyOfNewValidator, governingTokenBalance);

            await _repository.AddAccount(account);
        }

        private async Task UpdateValidatorVote(ECPoint pubKey, Fixed8 delta)
        {
            var validator = await _repository.GetValidator(pubKey);
            validator.Votes += delta;
            await _repository.AddValidator(validator);
        }
    }
}