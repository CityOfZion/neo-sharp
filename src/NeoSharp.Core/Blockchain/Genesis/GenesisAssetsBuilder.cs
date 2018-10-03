using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Models;
using NeoSharp.Core.Models.OperationManger;
using NeoSharp.Core.Network;
using NeoSharp.Core.SmartContract;
using NeoSharp.Types;
using NeoSharp.Types.ExtensionMethods;
using NeoSharp.VM;

namespace NeoSharp.Core.Blockchain.Genesis
{
    public class GenesisAssetsBuilder : IGenesisAssetsBuilder
    {
        #region Private fields 
        private const uint DecrementInterval = 2000000;
        private readonly uint[] _gasGenerationPerBlock = { 8, 7, 6, 5, 4, 3, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

        private readonly ISigner<Transaction> _transactionSigner;

        private readonly NetworkConfig _networkConfig;

        private RegisterTransaction _governingTokenRegisterTransaction;
        private RegisterTransaction _utilityTokenRegisterTransaction;
        #endregion

        #region Constructor 
        public GenesisAssetsBuilder(ISigner<Transaction> transactionSigner) 
        {
            this._transactionSigner = transactionSigner;

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true);

            var configuration = builder.Build();

            this._networkConfig = new NetworkConfig(configuration);
        }
        #endregion

        #region IGenesisAssets implementation 
        /// <inheritdoc />
        public RegisterTransaction BuildGoverningTokenRegisterTransaction()
        {
            // NEO Token is represented as a RegisterTransaction of type GoverningToken
            this._governingTokenRegisterTransaction = new RegisterTransaction
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

            this._transactionSigner.Sign(this._governingTokenRegisterTransaction);
            return this._governingTokenRegisterTransaction;
        }

        /// <inheritdoc />
        public RegisterTransaction BuildUtilityTokenRegisterTransaction()
        {
            // GAS Token is represented as a RegisterTransaction of type UtilityToken
            this._utilityTokenRegisterTransaction = new RegisterTransaction
            {
                AssetType = AssetType.UtilityToken,
                Name = "[{\"lang\":\"zh-CN\",\"name\":\"小蚁币\"},{\"lang\":\"en\",\"name\":\"AntCoin\"}]",
                Amount = Fixed8.FromDecimal(_gasGenerationPerBlock.Sum(p => p) * DecrementInterval),
                Precision = 8,
                Owner = ECPoint.Infinity,
                //Why this? Check with people
                Admin = (new[] { (byte)EVMOpCode.PUSH0 }).ToScriptHash(),
                Attributes = new TransactionAttribute[0],
                Inputs = new CoinReference[0],
                Outputs = new TransactionOutput[0],
                Witness = new Witness[0]
            };

            this._transactionSigner.Sign(this._utilityTokenRegisterTransaction);
            return this._utilityTokenRegisterTransaction;
        }

        /// <inheritdoc />
        public MinerTransaction BuildGenesisMinerTransaction()
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

        /// <inheritdoc />
        public IssueTransaction BuildGenesisIssueTransaction()
        {
            if (this._governingTokenRegisterTransaction == null) this.BuildGoverningTokenRegisterTransaction();
            if (this._utilityTokenRegisterTransaction == null) this.BuildUtilityTokenRegisterTransaction();

            var transactionOutput = this.GenesisGoverningTokenTransactionOutput();
            var genesisWitness = this.BuildGenesisWitness();
            var issueTransaction = new IssueTransaction
            {
                Attributes = new TransactionAttribute[0],
                Inputs = new CoinReference[0],
                Outputs = new[] { transactionOutput },
                Witness = new[] { genesisWitness }
            };

            return issueTransaction;
        }

        /// <inheritdoc />
        public Witness BuildGenesisWitness()
        {
            var witness = new Witness
            {
                InvocationScript = new byte[0],
                VerificationScript = new[] { (byte)EVMOpCode.PUSH1 }
            };

            return witness;
        }

        /// <inheritdoc />
        public UInt160 BuildGenesisNextConsensusAddress()
        {
            var genesisValidators = this.GenesisStandByValidators();
            return ContractFactory.CreateMultiplePublicKeyRedeemContract(genesisValidators.Length - (genesisValidators.Length - 1) / 3, genesisValidators).Code.ScriptHash;
        }
        #endregion

        #region Private Methods
        private ECPoint[] GenesisStandByValidators()
        {
            return this._networkConfig.StandByValidator.Select(u => new ECPoint(u.HexToBytes())).ToArray();
        }

        private TransactionOutput GenesisGoverningTokenTransactionOutput()
        {
            var genesisContract = this.GenesisValidatorsContract();

            var transactionOutput = new TransactionOutput
            {
                AssetId = this._governingTokenRegisterTransaction.Hash,
                Value = this._governingTokenRegisterTransaction.Amount,
                ScriptHash = genesisContract.ScriptHash
            };

            return transactionOutput;
        }

        private Contract GenesisValidatorsContract()
        {
            var genesisValidators = this.GenesisStandByValidators();
            var genesisContract = ContractFactory.CreateMultiplePublicKeyRedeemContract(genesisValidators.Length / 2 + 1, genesisValidators);
            return genesisContract;
        }
        #endregion
    }
}
