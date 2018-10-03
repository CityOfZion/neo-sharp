using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Core.Blockchain.Repositories;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.TestHelpers;
using NeoSharp.Types;

namespace NeoSharp.Core.Test.Blockchain
{
    [TestClass]
    public class UtBlockchain : TestBase
    {
        [TestMethod]
        public void Ctor_CreateValidBlockchainObject()
        {
            var testee = this.AutoMockContainer.Create<Core.Blockchain.Blockchain>();

            testee
                .Should()
                .BeOfType<Core.Blockchain.Blockchain>();
        }

        [TestMethod]
        public void IsDoubleSpend_NoInputs()
        {
            var testee = AutoMockContainer.Create<TransactionRepository>();

            var tx = new Transaction();
            
            var result = testee.IsDoubleSpend(tx);
            
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsDoubleSpend_InputWithoutFoundHash()
        {
            var testee = AutoMockContainer.Create<TransactionRepository>();

            var tx = new Transaction { Inputs = new[] { new CoinReference() } };
            tx.Inputs[0].PrevHash = new UInt256();
            
            var result = testee.IsDoubleSpend(tx);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsDoubleSpend_SpentInput()
        {
            var testee = AutoMockContainer.Create<TransactionRepository>();

            var tx = new Transaction { Inputs = new[] { new CoinReference() } };
            tx.Inputs[0].PrevHash = new UInt256(new byte[]{ 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 });
            
            // mocking: IRepository instance, when called GetCoinStates with our test hash,
            // returns a single spent coinstate
            var repositoryMock = AutoMockContainer.GetMock<IRepository>();
            repositoryMock
                .Setup(x => x.GetCoinStates(tx.Inputs[0].PrevHash))
                .ReturnsAsync(new[] { CoinState.Spent });
            
            var result = testee.IsDoubleSpend(tx);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsDoubleSpend_ConfirmedInput()
        {
            var testee = AutoMockContainer.Create<TransactionRepository>();

            var tx = new Transaction { Inputs = new[] { new CoinReference() } };
            tx.Inputs[0].PrevHash = new UInt256(new byte[]{ 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 });
            
            // mocking: IRepository instance, when called GetCoinStates with our test hash,
            // returns a single confirmed coinstate
            var repositoryMock = AutoMockContainer.GetMock<IRepository>();
            repositoryMock
                .Setup(x => x.GetCoinStates(tx.Inputs[0].PrevHash))
                .ReturnsAsync(new[] { CoinState.Confirmed });
            
            var result = testee.IsDoubleSpend(tx);

            result.Should().BeFalse();
        }

        //[TestMethod]
        //public async Task InitializeBlockchain_IsGenesisBlock_InitializeComplete()
        //{
        //    const uint expectedTotalBlockHeight = 0;
        //    const uint expectedTotalBlockHeaderHeight = 0;

        //    var genesisBlock = Genesis.GenesisBlock;

        //    var repositoryMock = this.AutoMockContainer.GetMock<IRepository>();

        //    repositoryMock
        //        .Setup(x => x.GetTotalBlockHeight())
        //        .ReturnsAsync(expectedTotalBlockHeight);
        //    repositoryMock
        //        .Setup(x => x.GetTotalBlockHeaderHeight())
        //        .ReturnsAsync(expectedTotalBlockHeaderHeight);
        //    repositoryMock
        //        .Setup(x => x.GetBlockHashFromHeight(expectedTotalBlockHeight))
        //        .ReturnsAsync((UInt256)null);
        //    repositoryMock
        //        .Setup(x => x.GetBlockHashFromHeight(expectedTotalBlockHeight))
        //        .ReturnsAsync((UInt256) null);

        //    var processorBlockMock = this.AutoMockContainer.GetMock<IProcessor<Block>>();

        //    var testee = this.AutoMockContainer.Create<Core.Blockchain.Blockchain>();
        //    await testee.InitializeBlockchain();

        //    testee.BlockPool.Count
        //        .Should()
        //        .Be(0);
        //    testee.MemoryPool.Count
        //        .Should()
        //        .Be(0);

        //    processorBlockMock
        //        .Verify(x => x.Process(genesisBlock));
        //    repositoryMock
        //        .Verify(x => x.SetTotalBlockHeaderHeight(genesisBlock.Index));

        //    // TODO #408: Cannot verify all the BlockPool logic of Push or PopFirstOrDefault. Suggestion to have a BlockPoolManager where this logic can be isolated and we can be sure the code is doing what is suppose to be doing.
        //    // TODO #409: Cannot veryfy all the MemPool login of Push or Remove. Suggestion to have a MemPoolManager where this logic can be isolated and we can be sure the code is doing what is suppose to be doing. 
        //}
    }
}
