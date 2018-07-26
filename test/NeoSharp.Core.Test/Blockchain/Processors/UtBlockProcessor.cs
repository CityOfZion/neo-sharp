using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Core.Blockchain.Processors;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Helpers;
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

        [TestMethod]
        public void Run_WhenAddBlockThisIsProcessed_OnBlockProcessedEventRaised()
        {
            var waitForBlockProcessedEvent = new AutoResetEvent(false);

            var currentBlock = new Block
            {
                Index = 0
            };

            var transactionInNewBlock = new Transaction();

            var newBlock = new Block
            {
                Hash = new UInt256("1a259dba256600620c6c91094f3a300b30f0cbaecee19c6114deffd3288957d7".HexToBytes()),
                Index = 1,
                Transactions = new[] { transactionInNewBlock }
            };

            var blockPoolMock = this.AutoMockContainer.GetMock<IBlockPool>();
            blockPoolMock
                .SetupGet(x => x.CurrentBlock)
                .Returns(currentBlock);
            blockPoolMock
                .Setup(x => x.TryGet(1, out newBlock))
                .Returns(true);

            var transactionProcessorMock = this.AutoMockContainer.GetMock<IProcessor<Transaction>>();
            var repositoryMock = this.AutoMockContainer.GetMock<IRepository>();

            var testee = this.AutoMockContainer.Create<BlockProcessor>();

            testee.OnBlockProcessed += block =>
            {
                block
                    .Should()
                    .BeSameAs(newBlock);

                waitForBlockProcessedEvent.Set();
                testee.Dispose();
                return Task.Run(() => { });
            };

            testee.Run(currentBlock);

            waitForBlockProcessedEvent.WaitOne();

            transactionProcessorMock.Verify(x => x.Process(transactionInNewBlock));

            repositoryMock.Verify(x => x.AddBlockHeader(It.Is<BlockHeader>(blockHeader => 
                blockHeader.ConsensusData == newBlock.ConsensusData &&
                blockHeader.Hash == newBlock.Hash &&
                blockHeader.Index == newBlock.Index &&
                blockHeader.Timestamp == newBlock.Timestamp &&
                blockHeader.Version == newBlock.Version)));
            repositoryMock.Verify(x => x.SetTotalBlockHeight(newBlock.Index));
        }

        [TestMethod]
        public void Run_BlockInPoolIsNotTheNext_AsyncDelayerCalledToWaitToReceiveCorrectNextBlock()
        {
            var waitForDelayForToGetNextBlock = new AutoResetEvent(false);

            var currentBlock = new Block
            {
                Index = 0
            };

            Block nullBlock = null;

            var blockPoolMock = this.AutoMockContainer.GetMock<IBlockPool>();
            blockPoolMock
                .SetupGet(x => x.CurrentBlock)
                .Returns(currentBlock);
            blockPoolMock
                .Setup(x => x.TryGet(1, out nullBlock))
                .Returns(false);

            var testee = this.AutoMockContainer.Create<BlockProcessor>();

            var asyncDelayerMock = this.AutoMockContainer.GetMock<IAsyncDelayer>();
            asyncDelayerMock
                .Setup(x => x.Delay(It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
                .Callback(() => { waitForDelayForToGetNextBlock.Set(); })
                .Returns(Task.Run(() => { }));
            
            testee.Run(currentBlock);
            waitForDelayForToGetNextBlock.WaitOne();
            testee.Dispose();

            asyncDelayerMock.Verify(x => x.Delay(It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()));
        }
    }
}