using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Core.Blockchain.Genesis;
using NeoSharp.Core.Blockchain.Processing.BlockHeaderProcessing;
using NeoSharp.Core.Network;
using NeoSharp.Core.Test.Builders;
using NeoSharp.TestHelpers;
using NeoSharp.Types;

namespace NeoSharp.Core.Test.Blockchain.Processing.BlockHeaderProcessing
{
    [TestClass]
    public class UtBlockHeaderValidator : TestBase
    {
        [TestMethod]
        public void Ctor_CreateValidBlockHeaderPersisterObject()
        {
            var testee = this.AutoMockContainer.Create<BlockHeaderValidator>();

            testee
                .Should()
                .BeOfType<BlockHeaderValidator>();
        }

        [TestMethod]
        public void IsValid_ValidGenesisBlockAndNoLastBlockHeader_IsValid()
        {
            // Arrange
            var blockHeaderToValidate = new BlockHeaderBuilder()
                .WithHash(UInt256.Parse("0xd42561e3d30e15be6400b6df2f328e02d2bf6354c41dce433bc57687c82144bf"))
                .WithIndex(0)
                .Build();

            var genesisBlock = new BlockBuilder()
                .BuildGenerisBlock();
            this.AutoMockContainer
                .GetMock<IGenesisBuilder>()
                .Setup(x => x.Build())
                .Returns(genesisBlock);

            var testee = this.AutoMockContainer.Create<BlockHeaderValidator>();

            // Act
            var result = testee.IsValid(blockHeaderToValidate);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsValid_InvalidGenesisBlockAndNoLastBlockHeader_IsNotInvalid()
        {
            // Arrange
            var blockHeaderToValidate = new BlockHeaderBuilder()
                .WithHash(UInt256.Parse("0xd42561e3d30e15be6400b6df2f328e02d2bf6354c41dce433bc57687c82144bf"))
                .WithIndex(1)
                .Build();

            var genesisBlock = new BlockBuilder()
                .BuildGenerisBlock();
            this.AutoMockContainer
                .GetMock<IGenesisBuilder>()
                .Setup(x => x.Build())
                .Returns(genesisBlock);

            var testee = this.AutoMockContainer.Create<BlockHeaderValidator>();

            // Act
            var result = testee.IsValid(blockHeaderToValidate);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsValid_ValidChainedBlock_IsValid()
        {
            // Arrange
            var blockHeaderToValidate = new BlockHeaderBuilder()
                .WithIndex(2)
                .WithPreviousBlockHash(UInt256.Parse("0xd42561e3d30e15be6400b6df2f328e02d2bf6354c41dce433bc57687c82144bf"))
                .Build();

            var lastBlockHeader = new BlockHeaderBuilder()
                .WithHash(UInt256.Parse("0xd42561e3d30e15be6400b6df2f328e02d2bf6354c41dce433bc57687c82144bf"))
                .WithIndex(1)
                .Build();

            this.AutoMockContainer
                .GetMock<IBlockchainContext>()
                .SetupGet(x => x.LastBlockHeader)
                .Returns(lastBlockHeader);

            var testee = this.AutoMockContainer.Create<BlockHeaderValidator>();

            // Act
            var result = testee.IsValid(blockHeaderToValidate);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsValid_InvalidChainedBlock_IsNotValid()
        {
            // Arrange
            var blockHeaderToValidate = new BlockHeaderBuilder()
                .WithIndex(3)
                .WithPreviousBlockHash(UInt256.Parse("0xd42561e3d30e15be6400b6df2f328e02d2bf6354c41dce433bc57687c82144bf"))
                .Build();

            var lastBlockHeader = new BlockHeaderBuilder()
                .WithHash(UInt256.Parse("0xd42561e3d30e15be6400b6df2f328e02d2bf6354c41dce433bc57687c82144bf"))
                .WithIndex(1)
                .Build();

            this.AutoMockContainer
                .GetMock<IBlockchainContext>()
                .SetupGet(x => x.LastBlockHeader)
                .Returns(lastBlockHeader);

            var testee = this.AutoMockContainer.Create<BlockHeaderValidator>();

            // Act
            var result = testee.IsValid(blockHeaderToValidate);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
