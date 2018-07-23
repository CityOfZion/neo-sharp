using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Core.Blockchain.Processors;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.Core.Types;
using NeoSharp.TestHelpers;

namespace NeoSharp.Core.Test.Blockchain.Processors
{
    [TestClass]
    public class UtBlockProcessor : TestBase
    {
        [TestMethod]
        public void Ctor_CreateValidBlockProcessorObject()
        {
            var testee = this.AutoMockContainer.Create<BlockProcessor>();

            testee
                .Should()
                .BeOfType<BlockProcessor>();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task AddBlock_BlockParameterIsNull_ThrowArgumentNullException()
        {
            var testee = this.AutoMockContainer.Create<BlockProcessor>();

            await testee.AddBlock(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task AddBlock_BlockHashIsNull_ThrowArgumentNullException()
        {
            var block = new Block();

            var blockPoolMock = this.AutoMockContainer.GetMock<IBlockPool>();
            blockPoolMock
                .Setup(x => x.Contains(block.Hash))
                .Returns(true);

            var testee = this.AutoMockContainer.Create<BlockProcessor>();

            await testee.AddBlock(block);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task AddBlock_BlockHashIsZero_ThrowArgumentException()
        {
            var block = new Block
            {
                Hash = UInt256.Zero
            };

            var testee = this.AutoMockContainer.Create<BlockProcessor>();

            await testee.AddBlock(block);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task AddBlock_ValidBlockButInBlockPool_ThrowInvalidOperationException()
        {
            var block = new Block
            {
                Hash = new UInt256("1a259dba256600620c6c91094f3a300b30f0cbaecee19c6114deffd3288957d7".HexToBytes())
            };

            var blockPoolMock = this.AutoMockContainer.GetMock<IBlockPool>();
            blockPoolMock
                .Setup(x => x.Contains(block.Hash))
                .Returns(true);

            var testee = this.AutoMockContainer.Create<BlockProcessor>();

            await testee.AddBlock(block);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task AddBlock_ValidBlockNotInBlockPoolInBlockchainButNotTheRightBlockHeaderType_ThrowInvalidOperationException()
        {
            var block = new Block
            {
                Hash = new UInt256("1a259dba256600620c6c91094f3a300b30f0cbaecee19c6114deffd3288957d7".HexToBytes())
            };

            var expectedBlockHeader = new BlockHeader(BlockHeader.HeaderType.Extended);

            var blockPoolMock = this.AutoMockContainer.GetMock<IBlockPool>();
            blockPoolMock
                .Setup(x => x.Contains(block.Hash))
                .Returns(false);

            var repositoryMock = this.AutoMockContainer.GetMock<IRepository>();
            repositoryMock
                .Setup(x => x.GetBlockHeader(block.Hash))
                .ReturnsAsync(expectedBlockHeader);

            var testee = this.AutoMockContainer.Create<BlockProcessor>();

            await testee.AddBlock(block);
        }

        [TestMethod]
        public async Task AddBlock_ValidBlockNotInBlockPoolInBlockChainWithTheRightBlockHeaderType_BlockAddedToBlockPool()
        {
            var block = new Block
            {
                Hash = new UInt256("1a259dba256600620c6c91094f3a300b30f0cbaecee19c6114deffd3288957d7".HexToBytes())
            };

            var expectedBlockHeader = new BlockHeader(BlockHeader.HeaderType.Header);

            var blockPoolMock = this.AutoMockContainer.GetMock<IBlockPool>();
            blockPoolMock
                .Setup(x => x.Contains(block.Hash))
                .Returns(false);

            var repositoryMock = this.AutoMockContainer.GetMock<IRepository>();
            repositoryMock
                .Setup(x => x.GetBlockHeader(block.Hash))
                .ReturnsAsync(expectedBlockHeader);

            var testee = this.AutoMockContainer.Create<BlockProcessor>();

            await testee.AddBlock(block);

            blockPoolMock
                .Verify(x => x.Add(block));
        }

        [TestMethod]
        public async Task AddBlock_ValidBlockNotInBlockPoolNotInBlockChain_BlockAddedToBlockPool()
        {
            var block = new Block
            {
                Hash = new UInt256("1a259dba256600620c6c91094f3a300b30f0cbaecee19c6114deffd3288957d7".HexToBytes())
            };

            var blockPoolMock = this.AutoMockContainer.GetMock<IBlockPool>();
            blockPoolMock
                .Setup(x => x.Contains(block.Hash))
                .Returns(false);

            var repositoryMock = this.AutoMockContainer.GetMock<IRepository>();
            repositoryMock
                .Setup(x => x.GetBlockHeader(block.Hash))
                .ReturnsAsync((BlockHeader)null);

            var testee = this.AutoMockContainer.Create<BlockProcessor>();

            await testee.AddBlock(block);

            blockPoolMock
                .Verify(x => x.Add(block));
        }

        //[TestMethod]
        //public void Process_CallsTransactionProcessorPerTransaction()
        //{
        //    var input = Genesis.GenesisBlock;
        //    var txProcessorMock = AutoMockContainer.GetMock<IProcessor<Transaction>>();
        //    var testee = AutoMockContainer.Create<BlockProcessor>();

        //    testee.AddBlock(input);
        //    txProcessorMock.Verify(m => m.Process(It.Is<Transaction>(t => input.Transactions.Contains(t))),
        //        Times.Exactly(input.Transactions.Length));
        //}


        //[TestMethod]
        //public void Process_PersistsBlockHeaderAndIndex()
        //{
        //    var expectedIndex = (uint) RandomInt();
        //    var input = new Block
        //    {
        //        Hash = UInt256.Parse(RandomInt().ToString("X64")),
        //        Index = expectedIndex,
        //        Transactions = new Transaction[0]
        //    };
        //    var repositoryMock = AutoMockContainer.GetMock<IRepository>();
        //    var testee = AutoMockContainer.Create<BlockProcessor>();

        //    testee.AddBlock(input);
        //    repositoryMock.Verify(m => m.SetTotalBlockHeight(expectedIndex));
        //    repositoryMock.Verify(m =>
        //        m.AddBlockHeader(It.Is<BlockHeader>(bh => bh.Index == input.Index && bh.Hash.Equals(input.Hash))));
        //}
    }
}