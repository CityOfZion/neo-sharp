using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Blockchain.Processing;
using NeoSharp.Core.DI;
using NeoSharp.Core.DI.Modules;
using NeoSharp.Core.Models;
using NeoSharp.Core.Models.OperationManger;
using NeoSharp.TestHelpers;

namespace NeoSharp.Application.Test
{
    [TestClass]
    public class UtBlockchainModule : TestBase
    {
        [TestMethod]
        public void Register_AllObjectsAreCorrectlyRegister()
        {
            // Arrange
            var containerBuilderMock = AutoMockContainer.GetMock<IContainerBuilder>();
            var module = AutoMockContainer.Create<BlockchainModule>();

            // Act
            module.Register(containerBuilderMock.Object);

            // Assert
            containerBuilderMock.Verify(x => x.RegisterSingleton<IBlockchain, Blockchain>(), Times.Once);
            containerBuilderMock.Verify(x => x.RegisterSingleton<IBlockProcessor, BlockProcessor>(), Times.Once);
            containerBuilderMock.Verify(x => x.RegisterSingleton<IBlockPool, BlockPool>(), Times.Once);
            containerBuilderMock.Verify(x => x.RegisterSingleton<ITransactionPersister<Transaction>, TransactionPersister>(), Times.Once);
            containerBuilderMock.Verify(x => x.RegisterSingleton<ITransactionPersister<InvocationTransaction>, InvocationTransactionPersister>(), Times.Once);

            containerBuilderMock.Verify(x => x.RegisterSingleton<IWitnessOperationsManager, WitnessOperationsManager>(), Times.Once);
            containerBuilderMock.Verify(x => x.RegisterSingleton<ITransactionOperationsManager, TransactionOperationManager>(), Times.Once);
            containerBuilderMock.Verify(x => x.RegisterSingleton<IBlockHeaderOperationsManager, BlockHeaderOperationsManager>(), Times.Once);
            containerBuilderMock.Verify(x => x.RegisterSingleton<IBlockOperationsManager, BlockOperationManager>(), Times.Once);
        }
    }
}