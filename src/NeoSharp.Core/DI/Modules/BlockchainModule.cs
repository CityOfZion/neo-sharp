using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Blockchain.Processors;
using NeoSharp.Core.Blockchain.State;
using NeoSharp.Core.Models;

namespace NeoSharp.Core.DI.Modules
{
    public class BlockchainModule : IModule
    {
        public void Register(IContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterSingleton<IBlockchain, Blockchain.Blockchain>();
            containerBuilder.RegisterSingleton<ICoinIndex, CoinIndex>();

            #region Processors

            containerBuilder.RegisterSingleton<IBlockProcessor, BlockProcessor>();
            containerBuilder.RegisterSingleton<IProcessor<Transaction>, TransactionProcessor>();

            containerBuilder.RegisterSingleton<IProcessor<ClaimTransaction>, ClaimTransactionProcessor>();
            containerBuilder.RegisterSingleton<IProcessor<InvocationTransaction>, InvocationTransactionProcessor>();
            containerBuilder.RegisterSingleton<IProcessor<IssueTransaction>, IssueTransactionProcessor>();
            containerBuilder.RegisterSingleton<IProcessor<PublishTransaction>, PublishTransactionProcessor>();
            containerBuilder.RegisterSingleton<IProcessor<RegisterTransaction>, RegisterTransactionProcessor>();
            containerBuilder.RegisterSingleton<IProcessor<StateTransaction>, StateTransactionProcessor>();
            containerBuilder.RegisterSingleton<IProcessor<EnrollmentTransaction>, EnrollmentTransactionProcessor>();

            #endregion

            containerBuilder.RegisterSingleton<IAccountManager, AccountManager>();
        }
    }
}