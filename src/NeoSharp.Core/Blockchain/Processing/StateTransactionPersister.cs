using System;
using System.Threading.Tasks;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Blockchain.State;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.Types;

namespace NeoSharp.Core.Blockchain.Processing
{
    public class StateTransactionPersister : ITransactionPersister<StateTransaction>
    {
        private readonly IRepository _repository;
        private readonly IAccountManager _accountManager;

        public StateTransactionPersister(IRepository repository, IAccountManager accountManager)
        {
            _repository = repository;
            _accountManager = accountManager;
        }

        public async Task Persist(StateTransaction stateTx)
        {
            foreach (var descriptor in stateTx.Descriptors)
                switch (descriptor.Type)
                {
                    case StateType.Account:
                        await ProcessAccountStateDescriptor(descriptor);
                        break;
                    case StateType.Validator:
                        await ProcessValidatorStateDescriptor(descriptor);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
        }

        private async Task ProcessAccountStateDescriptor(StateDescriptor descriptor)
        {
            var accountHash = new UInt160(descriptor.Key);

            switch (descriptor.Field)
            {
                case "Votes":
                    var chosenValidators = BinarySerializer.Default.Deserialize<ECPoint[]>(descriptor.Value);
                    await _accountManager.UpdateVotes(accountHash, chosenValidators);
                    break;
            }
        }

        private async Task ProcessValidatorStateDescriptor(StateDescriptor descriptor)
        {
            var pubKey = new ECPoint(descriptor.Key);
            var validator = await _repository.GetValidator(pubKey) ?? new Validator { PublicKey = pubKey };

            switch (descriptor.Field)
            {
                case "Registered":
                    validator.Registered = BitConverter.ToBoolean(descriptor.Value, 0);
                    break;
            }

            await _repository.AddValidator(validator);
        }
    }
}