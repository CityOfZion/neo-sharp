using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Core.Blockchain.Processing.BlockHeaderProcessing;
using NeoSharp.Core.Blockchain.Repositories;
using NeoSharp.Core.Models;
using NeoSharp.Core.Models.OperationManger;
using NeoSharp.Core.Network;
using NeoSharp.Core.Test.Builders;
using NeoSharp.TestHelpers;

namespace NeoSharp.Core.Test.Blockchain.Processing.BlockHeaderProcessing
{
    [TestClass]
    public class UtBlockHeaderPersister : TestBase
    {
        [TestMethod]
        public void Ctor_CreateValidBlockHeaderPersisterObject()
        {
            var testee = this.AutoMockContainer.Create<BlockHeaderPersister>();

            testee
                .Should()
                .BeOfType<BlockHeaderPersister>();
        }

        [TestMethod]
        public async Task Update_UpdateBlockHeaderCalledInBlockRepository()
        {
            // Arrange 
            var blockHeader = new BlockHeader();

            var blockRepositoryMock = this.AutoMockContainer.GetMock<IBlockRepository>();

            var testee = this.AutoMockContainer.Create<BlockHeaderPersister>();
            
            // Act
            await testee.Update(blockHeader);

            // Assert
            blockRepositoryMock.Verify(x => x.UpdateBlockHeader(blockHeader));
        }

        [TestMethod]
        public async Task Persist_ValidGenesisBlock_BlockIsPersisted()
        {
            // Arrange
            var blockHeaderToPersist = new BlockHeaderBuilder()
                .Build();

            var blockHeaderValidatorMock = this.AutoMockContainer.GetMock<IBlockHeaderValidator>();
            blockHeaderValidatorMock
                .Setup(x => x.IsValid(blockHeaderToPersist))
                .Returns(true);

            var blockContextMock = this.AutoMockContainer.GetMock<IBlockchainContext>();
            blockContextMock
                .SetupGet(x => x.LastBlockHeader)
                .Returns(default(BlockHeader));

            var blockHeaderSignerMock = this.AutoMockContainer.GetMock<ISigner<BlockHeader>>();
            var blockRepository = this.AutoMockContainer.GetMock<IBlockRepository>();

            var testee = this.AutoMockContainer.Create<BlockHeaderPersister>();
            
            // Act
            var persistedBlocks = (await testee.Persist(blockHeaderToPersist)).ToList();

            // Assert
            persistedBlocks
                .Should()
                .HaveCount(1);
            persistedBlocks
                .Single()
                .Should()
                .BeEquivalentTo(blockHeaderToPersist);

            blockHeaderSignerMock.Verify(x => x.Sign(blockHeaderToPersist), Times.Once);
            blockHeaderValidatorMock.Verify(x => x.IsValid(blockHeaderToPersist), Times.Once);
            blockRepository.Verify(x => x.AddBlockHeader(blockHeaderToPersist), Times.Once);
            blockContextMock.VerifySet(x => x.LastBlockHeader = blockHeaderToPersist, Times.Once);
        }

        [TestMethod]
        public async Task Persist_InvalidGenesisBlock_BlockIsNotPersisted()
        {
            // Arrange
            var blockHeaderToPersist = new BlockHeaderBuilder()
                .Build();

            var blockHeaderValidatorMock = this.AutoMockContainer.GetMock<IBlockHeaderValidator>();
            blockHeaderValidatorMock
                .Setup(x => x.IsValid(blockHeaderToPersist))
                .Returns(false);

            var blockContextMock = this.AutoMockContainer.GetMock<IBlockchainContext>();
            blockContextMock
                .SetupGet(x => x.LastBlockHeader)
                .Returns(default(BlockHeader));

            var blockHeaderSignerMock = this.AutoMockContainer.GetMock<ISigner<BlockHeader>>();
            var blockRepository = this.AutoMockContainer.GetMock<IBlockRepository>();

            var testee = this.AutoMockContainer.Create<BlockHeaderPersister>();

            // Act
            var persistedBlocks = (await testee.Persist(blockHeaderToPersist)).ToList();

            // Assert
            persistedBlocks
                .Should()
                .BeEmpty();

            blockHeaderSignerMock.Verify(x => x.Sign(blockHeaderToPersist), Times.Once);
            blockHeaderValidatorMock.Verify(x => x.IsValid(blockHeaderToPersist), Times.Once);
            blockRepository.Verify(x => x.AddBlockHeader(blockHeaderToPersist), Times.Never);
            blockContextMock.VerifySet(x => x.LastBlockHeader = blockHeaderToPersist, Times.Never);
        }

        [TestMethod]
        public async Task Persist_ValidBlock_BlockIsPersisted()
        {
            // Arrange
            var blockHeaderToPersist = new BlockHeaderBuilder()
                .WithIndex(2)
                .Build();

            var lastBlockHeader = new BlockHeaderBuilder()
                .WithIndex(1)
                .Build();

            var blockHeaderValidatorMock = this.AutoMockContainer.GetMock<IBlockHeaderValidator>();
            blockHeaderValidatorMock
                .Setup(x => x.IsValid(blockHeaderToPersist))
                .Returns(true);

            var blockContextMock = this.AutoMockContainer.GetMock<IBlockchainContext>();
            blockContextMock
                .SetupGet(x => x.LastBlockHeader)
                .Returns(lastBlockHeader);

            var blockHeaderSignerMock = this.AutoMockContainer.GetMock<ISigner<BlockHeader>>();
            var blockRepository = this.AutoMockContainer.GetMock<IBlockRepository>();

            var testee = this.AutoMockContainer.Create<BlockHeaderPersister>();

            // Act
            var persistedBlocks = (await testee.Persist(blockHeaderToPersist)).ToList();

            // Assert
            persistedBlocks
                .Should()
                .HaveCount(1);
            persistedBlocks
                .Single()
                .Should()
                .BeEquivalentTo(blockHeaderToPersist);

            blockHeaderSignerMock.Verify(x => x.Sign(blockHeaderToPersist), Times.Once);
            blockHeaderValidatorMock.Verify(x => x.IsValid(blockHeaderToPersist), Times.Once);
            blockRepository.Verify(x => x.AddBlockHeader(blockHeaderToPersist), Times.Once);
            blockContextMock.VerifySet(x => x.LastBlockHeader = blockHeaderToPersist, Times.Once);
        }

        [TestMethod]
        public async Task Persist_ValidBlockFromPast_BlockIsNotPersisted()
        {
            // Arrange
            var blockHeaderToPersist = new BlockHeaderBuilder()
                .WithIndex(1)
                .Build();

            var lastBlockHeader = new BlockHeaderBuilder()
                .WithIndex(2)
                .Build();

            var blockHeaderValidatorMock = this.AutoMockContainer.GetMock<IBlockHeaderValidator>();
            blockHeaderValidatorMock
                .Setup(x => x.IsValid(blockHeaderToPersist))
                .Returns(true);

            var blockContextMock = this.AutoMockContainer.GetMock<IBlockchainContext>();
            blockContextMock
                .SetupGet(x => x.LastBlockHeader)
                .Returns(lastBlockHeader);

            var blockHeaderSignerMock = this.AutoMockContainer.GetMock<ISigner<BlockHeader>>();
            var blockRepository = this.AutoMockContainer.GetMock<IBlockRepository>();

            var testee = this.AutoMockContainer.Create<BlockHeaderPersister>();

            // Act
            var persistedBlocks = (await testee.Persist(blockHeaderToPersist)).ToList();

            // Assert
            persistedBlocks
                .Should()
                .BeEmpty();

            blockHeaderSignerMock.Verify(x => x.Sign(blockHeaderToPersist), Times.Never);
            blockHeaderValidatorMock.Verify(x => x.IsValid(blockHeaderToPersist), Times.Never);
            blockRepository.Verify(x => x.AddBlockHeader(blockHeaderToPersist), Times.Never);
            blockContextMock.VerifySet(x => x.LastBlockHeader = blockHeaderToPersist, Times.Never);
        }

    // TODO [AboimPinto]: Because we cannot verify the order of the calls, will not be possible to verify the ordering of the BlockHeaders in case we we receive they out-of-order
    }
}
