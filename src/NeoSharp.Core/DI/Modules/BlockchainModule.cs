using System.Collections.Generic;
using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Blockchain.Genesis;
using NeoSharp.Core.Blockchain.Processing;
using NeoSharp.Core.Blockchain.Repositories;
using NeoSharp.Core.Blockchain.State;
using NeoSharp.Core.Models;
using NeoSharp.Core.Models.OperationManger;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.DI.Modules
{
    public class BlockchainModule : IModule
    {
        public void Register(IContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterSingleton<IGenesisBuilder, GenesisBuilder>();
            containerBuilder.RegisterSingleton<IGenesisAssetsBuilder, GenesisAssetsBuilder>();

            containerBuilder.RegisterSingleton<IBlockchain, Blockchain.Blockchain>();
            containerBuilder.RegisterSingleton<ICoinIndex, CoinIndex>();

            #region Processing
            containerBuilder.RegisterSingleton<IBlockPersister, BlockPersister>();
            containerBuilder.RegisterSingleton<IBlockHeaderPersister, BlockHeaderPersister>();
            containerBuilder.RegisterSingleton<IBlockProcessor, BlockProcessor>();
            containerBuilder.RegisterSingleton<IBlockPool, BlockPool>();
            containerBuilder.RegisterSingleton<IComparer<Transaction>, TransactionComparer>();
            containerBuilder.RegisterSingleton<ITransactionPool, TransactionPool>();
            containerBuilder.RegisterSingleton<ITransactionContext, TransactionContext>();
            containerBuilder.RegisterSingleton<ITransactionPersister<Transaction>, TransactionPersister>();

            containerBuilder.RegisterSingleton<ITransactionPersister<ClaimTransaction>, ClaimTransactionPersister>();
            containerBuilder.RegisterSingleton<ITransactionPersister<InvocationTransaction>, InvocationTransactionPersister>();
            containerBuilder.RegisterSingleton<ITransactionPersister<IssueTransaction>, IssueTransactionPersister>();
            containerBuilder.RegisterSingleton<ITransactionPersister<PublishTransaction>, PublishTransactionPersister>();
            containerBuilder.RegisterSingleton<ITransactionPersister<RegisterTransaction>, RegisterTransactionPersister>();
            containerBuilder.RegisterSingleton<ITransactionPersister<StateTransaction>, StateTransactionPersister>();
            containerBuilder.RegisterSingleton<ITransactionPersister<EnrollmentTransaction>, EnrollmentTransactionPersister>();

            containerBuilder.RegisterSingleton<IWitnessOperationsManager, WitnessOperationsManager>();
            containerBuilder.RegisterSingleton<ITransactionOperationsManager, TransactionOperationManager>();
            containerBuilder.RegisterSingleton<IBlockHeaderOperationsManager, BlockHeaderOperationsManager>();
            containerBuilder.RegisterSingleton<IBlockSigner, BlockSigner>();
            containerBuilder.RegisterSingleton<IBlockVerifier, BlockVerifier>();
            #endregion

            containerBuilder.RegisterSingleton<IAccountManager, AccountManager>();

            containerBuilder.RegisterSingleton<ITransactionRepository, TransactionRepository>();
            containerBuilder.RegisterSingleton<IAssetRepository, AssetRepository>();
            containerBuilder.RegisterSingleton<IBlockRepository, BlockRepository>();
        }
    }
}