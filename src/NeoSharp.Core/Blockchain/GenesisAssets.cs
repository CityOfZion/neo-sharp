using System;
using System.Linq;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Models;
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

        /// <summary>
        /// NEO Token is represented as a RegisterTransaction of type GoverningToken
        /// </summary>
        /// <returns>The NEO RegisterTransaction.</returns>
        /// TODO: Review static usage. Use singleton instead?
        public static RegisterTransaction GoverningTokenRegisterTransaction()
        {
            var neoToken = new RegisterTransaction
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
                Scripts = new Witness[0]
            };

            neoToken.UpdateHash(BinarySerializer.Default, ICrypto.Default);

            return neoToken;
        }

        /// <summary>
        /// GAS Token is represented as a RegisterTransaction of type UtilityToken
        /// </summary>
        /// <returns>The Gas RegisterTransaction.</returns>
        /// TODO: Review static usage. Use singleton instead?
        public static RegisterTransaction UtilityTokenRegisterTransaction()
        {
            var gasToken = new RegisterTransaction
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
                Scripts = new Witness[0]
            };

            gasToken.UpdateHash(BinarySerializer.Default, ICrypto.Default);

            return gasToken;
        }

        /// <summary>
        /// This transaction sends the initial NEO Amount to a contract that
        /// controlled by the seed validators.
        /// </summary>
        /// <returns>The genesis issue transaction.</returns>
        public static IssueTransaction GenesisIssueTransaction()
        {
            var governingToken = GoverningTokenRegisterTransaction();
            var transactionOutput = GenesisGoverningTokenTransactionOutput();
            var genesisWitness = GenesisWitness();
            var issueTransaction = new IssueTransaction
            {
                Attributes = new TransactionAttribute[0],
                Inputs = new CoinReference[0],
                Outputs = new[]{
                    transactionOutput
                },
                Scripts = new []{
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
                Scripts = new Witness[0]
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

        public static UInt160 GetGenesisNextConsensusAdress(){
            var genesisContract = GenesisValidatorsContract();
            return genesisContract.ScriptHash;
        }

        private static ECPoint[] GenesisStandByValidators(){
            // TODO: Extract from config
            var standByValidators = new string[]
                {
                "03b209fd4f53a7170ea4444e0cb0a6bb6a53c2bd016926989cf85f9b0fba17a70c",
                "02df48f60e8f3e01c48ff40b9b7f1310d7a8b2a193188befe1c2e3df740e895093",
                "03b8d9d5771d8f513aa0869b9cc8d50986403b78c6da36890638c3d46a5adce04a",
                "02ca0e27697b9c248f6f16e085fd0061e26f44da85b58ee835c110caa5ec3ba554",
                "024c7b7fb6c310fccf1ba33b082519d82964ea93868d676662d4a59ad548df0e7d",
                "02aaec38470f6aad0042c6e877cfd8087d2676b0f516fddd362801b9bd3936399e",
                "02486fd15702c4490a26703112a5cc1d0923fd697a33406bd5a1c00e0013b09a70"
                }.Select(u => new ECPoint(u.HexToBytes())).ToArray();

            return standByValidators;
        }

        /// <summary>
        /// The first transaction sends all NEO to contract 'managed' by 2/3 + 1 of the validators.
        /// </summary>
        /// <returns>The governing token transaction output.</returns>
        private static TransactionOutput GenesisGoverningTokenTransactionOutput()
        {
            
            var governingToken = GoverningTokenRegisterTransaction();
            var genesisContract = GenesisValidatorsContract();

            var transactionOutput = new TransactionOutput
            {
                AssetId = governingToken.Hash,
                Value = governingToken.Amount,
                ScriptHash = genesisContract.ScriptHash
            };

            return transactionOutput;
        }

        /// <summary>
        /// This is a Multisignature contract that requires 2/3 + 1 of the validators signatures
        /// </summary>
        /// <returns>The validators contract.</returns>
        private static Contract GenesisValidatorsContract(){
            var contractHelper = new ContractHelper();
            var genesisValidators = GenesisStandByValidators();
            var genesisContract = contractHelper.CreateMultiplePublicKeyRedeemContract(genesisValidators.Length / 2 + 1, genesisValidators);
            return genesisContract;
        }


    }
}
