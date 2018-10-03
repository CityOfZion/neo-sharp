using System;
using System.Collections.Generic;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.Types;

namespace NeoSharp.Core.Wallet.TransactionManager
{

    // assetId, unspent list
    using UnspentCoinsDictionary = Dictionary<UInt256, List<CoinReference>>;

    public class TransactionManager : ITransactionManager
    {

        IRepository _repository;

        public TransactionManager(IRepository repository){
            _repository = repository;
        }

        /// <inheritdoc />
        public ClaimTransaction BuildClaimTransaction(IWallet from, TransactionAttribute[] attributes){
            ClaimTransaction transaction = new ClaimTransaction();
            transaction.Attributes = attributes ?? new TransactionAttribute[0];
            //Exclusive Data
            //transaction.Claims = _repository.GetUnspent();


            return transaction;
        }


        /// <inheritdoc />
        public ClaimTransaction BuildClaimTransaction(CoinReference[] inputs, TransactionAttribute[] attributes)
        {
            if(inputs == null || inputs.Length == 0){
                throw new ArgumentException();
            }

            ClaimTransaction transaction = new ClaimTransaction();
            transaction.Inputs = inputs;
            transaction.Attributes = attributes ?? new TransactionAttribute[0];
            //TODO #395: Complete transaction

            return transaction;
        }

        /// <inheritdoc />
        public ClaimTransaction BuildClaimTransaction(TransactionAttribute[] attributes, CoinReference[] inputs)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ContractTransaction BuildContractTransaction(
            IWalletAccount from,
            TransactionAttribute[] attributes,
            TransactionOutput[] outputs){

            //TODO #396: Complete transaction
            return new ContractTransaction();
        }


        /// <inheritdoc />
        public ContractTransaction BuildContractTransaction(
            TransactionAttribute[] attributes,
            CoinReference[] inputs,
            TransactionOutput[] outputs){

            //TODO #396: Complete transaction
            return new ContractTransaction();
        }

        /// <inheritdoc />
        public ContractTransaction BuildContractTransaction(IWallet from, TransactionAttribute[] attributes, TransactionOutput[] outputs){
            return new ContractTransaction();
        }

        /// <inheritdoc />
        public InvocationTransaction BuildInvocationTransaction(IWalletAccount from, TransactionAttribute[] attributes, TransactionOutput[] outputs, string script, Fixed8 fee = default(Fixed8)){
            //TODO #397: Complete transaction
            return new InvocationTransaction();
        }

        /// <inheritdoc />
        public InvocationTransaction BuildInvocationTransaction(TransactionAttribute[] attributes, CoinReference[] inputs, TransactionOutput[] outputs, string script, Fixed8 fee = default(Fixed8)){
            //TODO #397: Complete transaction
            return new InvocationTransaction();
        }

        public UnspentCoinsDictionary GetBalance(IWalletAccount from)
        {
            throw new NotImplementedException();
        }

        public UInt256 BroadcastTransaction(IWalletAccount account, Transaction transaction)
        {
            throw new NotImplementedException();
        }

    }
}