using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Blockchain.Processing;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Helpers;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.Core.Types;
using NeoSharp.TestHelpers;

namespace NeoSharp.Core.Test.Blockchain.Processing
{
    [TestClass]
    public class UtBlockProcessor : TestBase
    {
        public UtBlockProcessor()
        {
            BinarySerializer.RegisterTypes(typeof(Block).Assembly);
        }

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
        [ExpectedException(typeof(ArgumentException))]
        public async Task AddBlock_BlockHashIsNull_ThrowArgumentException()
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
                PreviousBlockHash = UInt256.Zero,
                Hash = UInt256.Parse("d4dab99ed65c3655a9619b215ab1988561b706b6e5196b6e0ada916aa6601622"),
                NextConsensus = UInt160.Zero,
                Transactions = new Transaction[]
                {
                    new ContractTransaction
                    {
                        Hash = UInt256.Parse("1a259dba256600620c6c91094f3a300b30f0cbaecee19c6114deffd3288957d7")
                    }
                }
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
                PreviousBlockHash = UInt256.Zero,
                Hash = UInt256.Parse("d4dab99ed65c3655a9619b215ab1988561b706b6e5196b6e0ada916aa6601622"),
                NextConsensus = UInt160.Zero,
                Transactions = new Transaction[]
                {
                    new ContractTransaction
                    {
                        Hash = UInt256.Parse("1a259dba256600620c6c91094f3a300b30f0cbaecee19c6114deffd3288957d7")
                    }
                }
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
                PreviousBlockHash = UInt256.Zero,
                Hash = UInt256.Parse("d4dab99ed65c3655a9619b215ab1988561b706b6e5196b6e0ada916aa6601622"),
                NextConsensus = UInt160.Zero,
                Transactions = new Transaction[]
                {
                    new ContractTransaction
                    {
                        Hash = UInt256.Parse("1a259dba256600620c6c91094f3a300b30f0cbaecee19c6114deffd3288957d7")
                    }
                }
            };

            var expectedBlockHeader = new BlockHeader(BlockHeader.HeaderType.Header) { Hash = block.Hash };

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
                PreviousBlockHash = UInt256.Zero,
                Hash = UInt256.Parse("d4dab99ed65c3655a9619b215ab1988561b706b6e5196b6e0ada916aa6601622"),
                NextConsensus = UInt160.Zero,
                Transactions = new Transaction[]
                {
                    new ContractTransaction
                    {
                        Hash = UInt256.Parse("1a259dba256600620c6c91094f3a300b30f0cbaecee19c6114deffd3288957d7")
                    }
                }
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
                .Setup(x => x.TryGet(1, out newBlock))
                .Returns(true);

            var blockHeaderPersisterMock = this.AutoMockContainer.GetMock<IBlockHeaderPersister>();
            var transactionProcessorMock = this.AutoMockContainer.GetMock<ITransactionPersister<Transaction>>();
            var repositoryMock = this.AutoMockContainer.GetMock<IRepository>();

            var testee = this.AutoMockContainer.Create<BlockProcessor>();

            testee.OnBlockProcessed += (_, block) =>
            {
                block
                    .Should()
                    .BeSameAs(newBlock);

                waitForBlockProcessedEvent.Set();
                testee.Dispose();
            };

            testee.Run(currentBlock);

            waitForBlockProcessedEvent.WaitOne();

            transactionProcessorMock.Verify(x => x.Persist(transactionInNewBlock));

            blockHeaderPersisterMock.Verify(x => x.Persist(It.Is<BlockHeader>(blockHeader => 
                blockHeader.ConsensusData == newBlock.ConsensusData &&
                blockHeader.Hash == newBlock.Hash &&
                blockHeader.Index == newBlock.Index &&
                blockHeader.Timestamp == newBlock.Timestamp &&
                blockHeader.Version == newBlock.Version)));
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