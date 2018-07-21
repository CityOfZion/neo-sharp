using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Blockchain.Processors;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.Core.Types;
using NeoSharp.TestHelpers;

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

        //    // TODO: Cannot verify all the BlockPool logic of Push or PopFirstOrDefault. Suggestion to have a BlockPoolManager where this logic can be isolated and we can be sure the code is doing what is suppose to be doing.
        //    // TODO: Cannot veryfy all the MemPool login of Push or Remove. Suggestion to have a MemPoolManager where this logic can be isolated and we can be sure the code is doing what is suppose to be doing. 
        //}
    }
}
