using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Models;
using NeoSharp.Core.Network;
using NeoSharp.Core.SmartContract;
using NeoSharp.Core.Types;
using NeoSharp.Core.Wallet.Helpers;
using NeoSharp.VM;

namespace NeoSharp.Core.Blockchain
{
#pragma warning disable CS0612 // Type or member is obsolete
    public class GenesisAssets
    {
        const uint DecrementInterval = 2000000;
        static readonly uint[] GasGenerationPerBlock = { 8, 7, 6, 5, 4, 3, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        private static NetworkConfig _networkConfig;

        public static RegisterTransaction GoverningTokenRegisterTransaction { private set; get; }
        public static RegisterTransaction UtilityTokenRegisterTransaction { private set; get; }

        static GenesisAssets() 
        {
            // NEO Token is represented as a RegisterTransaction of type GoverningToken
            GoverningTokenRegisterTransaction = new RegisterTransaction
            {
                AssetType = AssetType.GoverningToken,
                Name = "[{\"lang\":\"zh-CN\",\"name\":\"小蚁股\"},{\"lang\":\"en\",\"name\":\"AntShare\"}]",
                Amount = Fixed8.FromDecimal(100000000),
                Precision = 0,
                Owner = ECPoint.Infinity,
                //Why this? Check with people
                Admin = (new[] { (byte)EVMOpCode.PUSH1 }).ToScriptHash(),
                Attributes = new TransactionAttribute[0],
                Inputs = new CoinReference[0],
                Outputs = new TransactionOutput[0],
                Witness = new Witness[0]
            };

            GoverningTokenRegisterTransaction.UpdateHash(BinarySerializer.Default, ICrypto.Default);

            // GAS Token is represented as a RegisterTransaction of type UtilityToken
            UtilityTokenRegisterTransaction = new RegisterTransaction
            {
                AssetType = AssetType.UtilityToken,
                Name = "[{\"lang\":\"zh-CN\",\"name\":\"小蚁币\"},{\"lang\":\"en\",\"name\":\"AntCoin\"}]",
                Amount = Fixed8.FromDecimal(GasGenerationPerBlock.Sum(p => p) * DecrementInterval),
                Precision = 8,
                Owner = ECPoint.Infinity,
                //Why this? Check with people
                Admin = (new[] { (byte)EVMOpCode.PUSH0 }).ToScriptHash(),
                Attributes = new TransactionAttribute[0],
                Inputs = new CoinReference[0],
                Outputs = new TransactionOutput[0],
                Witness = new Witness[0]
            };

            UtilityTokenRegisterTransaction.UpdateHash(BinarySerializer.Default, ICrypto.Default);

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true);

            var configuration = builder.Build();

            _networkConfig = new NetworkConfig(configuration);
        }

        /// <summary>
        /// This transaction sends the initial NEO Amount to a contract that
        /// controlled by the seed validators.
        /// </summary>
        /// <returns>The genesis issue transaction.</returns>
        public static IssueTransaction GenesisIssueTransaction()
        {
            var transactionOutput = GenesisGoverningTokenTransactionOutput();
            var genesisWitness = GenesisWitness();
            var issueTransaction = new IssueTransaction
            {
                Attributes = new TransactionAttribute[0],
                Inputs = new CoinReference[0],
                Outputs = new[]{
                    transactionOutput
                },
                Witness = new[]{
                    genesisWitness
                }

            };

            return issueTransaction;
        }

        /// <summary>
        /// The first miner transaction
        /// </summary>
        /// <returns>The miner transaction.</returns>
        public static MinerTransaction GenesisMinerTransaction()
        {
            uint genesisMinerNonce = 2083236893;
            var genesisMinerTransaction = new MinerTransaction
            {
                Nonce = genesisMinerNonce,
                Attributes = new TransactionAttribute[0],
                Inputs = new CoinReference[0],
                Outputs = new TransactionOutput[0],
                Witness = new Witness[0]
            };

            return genesisMinerTransaction;
        }

        /// <summary>
        /// TODO: Check with team how to document this
        /// </summary>
        /// <returns>The witness.</returns>
        public static Witness GenesisWitness()
        {
            var witness = new Witness
            {
                InvocationScript = new byte[0],
                VerificationScript = new[] { (byte)EVMOpCode.PUSH1 }
            };

            return witness;
        }

        public static UInt160 GetGenesisNextConsensusAdress()
        {
            var genesisValidators = GenesisStandByValidators();
            return ContractFactory.CreateMultiplePublicKeyRedeemContract(genesisValidators.Length - (genesisValidators.Length - 1) / 3, genesisValidators).Code.ScriptHash;
        }

        private static ECPoint[] GenesisStandByValidators(){
            return _networkConfig.StandByValidator.Select(u => new ECPoint(u.HexToBytes())).ToArray();
        }

        /// <summary>
        /// The first transaction sends all NEO to contract 'managed' by 2/3 + 1 of the validators.
        /// </summary>
        /// <returns>The governing token transaction output.</returns>
        private static TransactionOutput GenesisGoverningTokenTransactionOutput()
        {
            var genesisContract = GenesisValidatorsContract();

            var transactionOutput = new TransactionOutput
            {
                AssetId = GoverningTokenRegisterTransaction.Hash,
                Value = GoverningTokenRegisterTransaction.Amount,
                ScriptHash = genesisContract.ScriptHash
            };

            return transactionOutput;
        }

        /// <summary>
        /// This is a Multisignature contract that requires 2/3 + 1 of the validators signatures
        /// </summary>
        /// <returns>The validators contract.</returns>
        private static Contract GenesisValidatorsContract()
        {
            var genesisValidators = GenesisStandByValidators();
            var genesisContract = ContractFactory.CreateMultiplePublicKeyRedeemContract(genesisValidators.Length / 2 + 1, genesisValidators);
            return genesisContract;
        }
    }
}
