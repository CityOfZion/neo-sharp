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

            containerBuilder.RegisterSingleton<IProcessor<Block>, BlockProcessor>();
            containerBuilder.RegisterSingleton<IProcessor<Transaction>, TransactionProcessor>();
            containerBuilder.RegisterSingleton<IProcessor<ClaimTransaction>, ClaimTransactionProcessor>();
            containerBuilder.RegisterSingleton<IProcessor<InvocationTransaction>, InvocationTransactionProcessor>();

            #endregion

            containerBuilder.RegisterSingleton<IAccountManager, AccountManager>();
        }
    }
}