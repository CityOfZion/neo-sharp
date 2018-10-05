using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Blockchain.Genesis;
using NeoSharp.Core.Blockchain.Repositories;
using NeoSharp.Core.Models;
using NeoSharp.Core.Models.OperationManger;
using NeoSharp.Core.Types;
using NeoSharp.Cryptography;
using NeoSharp.TestHelpers;

namespace NeoSharp.Core.Test.Blockchain
{
    [TestClass]
    public class UtGenesis : TestBase
    {
        [TestMethod]
        public void GenesisHashTest()
        {
            BinarySerializer.RegisterTypes(typeof(Block).Assembly);

            var crypto = Crypto.Default;
            var binarySerialier = BinarySerializer.Default;
            var witnessOperationManager = new WitnessOperationsManager(crypto);
            var transactionOperationManager = new TransactionOperationManager(crypto, binarySerialier, witnessOperationManager, new Mock<ITransactionRepository>().Object, new Mock<IAssetRepository>().Object, new TransactionContext());
            var blockOperationManager = new BlockOperationManager(crypto, binarySerialier, transactionOperationManager, witnessOperationManager, new Mock<IBlockRepository>().Object);

            this.AutoMockContainer.Register(crypto);
            this.AutoMockContainer.Register(binarySerialier);
            this.AutoMockContainer.Register<IWitnessOperationsManager>(witnessOperationManager);
            this.AutoMockContainer.Register<ITransactionOperationsManager>(transactionOperationManager);
            this.AutoMockContainer.Register<ISigner<Block>>(blockOperationManager);
            this.AutoMockContainer.Register<ISigner<Transaction>>(transactionOperationManager);

            var genesisAssets = this.AutoMockContainer.Create<GenesisAssetsBuilder>();
            this.AutoMockContainer.Register<IGenesisAssetsBuilder>(genesisAssets);

            // TODO: this test need to be refactor.
            var genesis = this.AutoMockContainer.Create<GenesisBuilder>();
            Assert.AreEqual(genesis.Build().Hash.ToString(true), "0xd42561e3d30e15be6400b6df2f328e02d2bf6354c41dce433bc57687c82144bf");

            Assert.AreEqual(genesisAssets.BuildGoverningTokenRegisterTransaction().Hash.ToString(true), "0xc56f33fc6ecfcd0c225c4ab356fee59390af8560be0e930faebe74a6daff7c9b");
            Assert.AreEqual(genesisAssets.BuildUtilityTokenRegisterTransaction().Hash.ToString(true), "0x602c79718b16e442de58778e148d0b1084e3b2dffd5de6b7b16cee7969282de7");
        }
    }
}