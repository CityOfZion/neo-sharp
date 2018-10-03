using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Core.Blockchain.Repositories;
using NeoSharp.Core.Models;
using NeoSharp.Core.Models.OperationManger;
using NeoSharp.TestHelpers;
using NeoSharp.Types;

namespace NeoSharp.Core.Test.Models
{
    [TestClass]
    public class UtBlockVerifier : TestBase
    {
        
        [TestMethod]
        public void Verify_WithPrevBlockHeaderNotFound()
        {
            var testee = AutoMockContainer.Create<BlockOperationManager>();

            var block = new Block
            {
                PreviousBlockHash = UInt256.Parse("7ee8170d86de43d6c105699273f9b82d077180e5e0f8e4d942f43d7804cc54d3")
            };
            
            this.AutoMockContainer
                .GetMock<IBlockRepository>()
                .Setup(b => b.GetBlockHeader(block.PreviousBlockHash))
                .ReturnsAsync(() => null);
            
            var result = testee.Verify(block);
            
            result.Should().BeFalse();
        }
        
        [TestMethod]
        public void Verify_WithPrevBlockHeaderIndexNotThePrevious()
        {
            var testee = AutoMockContainer.Create<BlockOperationManager>();

            var block = new Block
            {
                PreviousBlockHash = UInt256.Parse("7ee8170d86de43d6c105699273f9b82d077180e5e0f8e4d942f43d7804cc54d3"),
                Index = 3
            };

            var prevBlockHeader = new BlockHeader
            {
                Index = 1
            };
            
            this.AutoMockContainer
                .GetMock<IBlockRepository>()
                .Setup(b => b.GetBlockHeader(block.PreviousBlockHash))
                .ReturnsAsync(() => prevBlockHeader);
            
            var result = testee.Verify(block);
            
            result.Should().BeFalse();
        }
        
        [TestMethod]
        public void Verify_WithPrevBlockHeaderGreaterTimestamp()
        {
            var testee = AutoMockContainer.Create<BlockOperationManager>();

            var block = new Block
            {
                PreviousBlockHash = UInt256.Parse("7ee8170d86de43d6c105699273f9b82d077180e5e0f8e4d942f43d7804cc54d3"),
                Index = 3,
                Timestamp = 111
            };

            var prevBlockHeader = new BlockHeader
            {
                Index = 2,
                Timestamp = 112
            };

            this.AutoMockContainer
                .GetMock<IBlockRepository>()
                .Setup(b => b.GetBlockHeader(block.PreviousBlockHash))
                .ReturnsAsync(() => prevBlockHeader);

            var result = testee.Verify(block);
            
            result.Should().BeFalse();
        }
        
        [TestMethod]
        public void Verify_WithPrevBlockHeaderVerifyWitnessFail()
        {
            var testee = AutoMockContainer.Create<BlockOperationManager>();

            var block = new Block
            {
                PreviousBlockHash = UInt256.Parse("7ee8170d86de43d6c105699273f9b82d077180e5e0f8e4d942f43d7804cc54d3"),
                Index = 3,
                Timestamp = 111,
                Witness = new Witness()
            };

            var prevBlockHeader = new BlockHeader
            {
                Index = 2,
                Timestamp = 110
            };

            this.AutoMockContainer
                .GetMock<IBlockRepository>()
                .Setup(b => b.GetBlockHeader(block.PreviousBlockHash))
                .ReturnsAsync(() => prevBlockHeader);

            this.AutoMockContainer
                .GetMock<IWitnessOperationsManager>()
                .Setup(b => b.Verify(block.Witness))
                .Returns(() => false);
            
            var result = testee.Verify(block);
            
            result.Should().BeFalse();
        }
        
        [TestMethod]
        public void Verify_Success()
        {
            var testee = AutoMockContainer.Create<BlockOperationManager>();

            var block = new Block
            {
                PreviousBlockHash = UInt256.Parse("7ee8170d86de43d6c105699273f9b82d077180e5e0f8e4d942f43d7804cc54d3"),
                Index = 3,
                Timestamp = 111,
                Witness = new Witness()
            };

            var prevBlockHeader = new BlockHeader
            {
                Index = 2,
                Timestamp = 110
            };

            this.AutoMockContainer
                .GetMock<IBlockRepository>()
                .Setup(b => b.GetBlockHeader(block.PreviousBlockHash))
                .ReturnsAsync(() => prevBlockHeader);

            this.AutoMockContainer
                .GetMock<IWitnessOperationsManager>()
                .Setup(b => b.Verify(block.Witness))
                .Returns(() => true);
            
            var result = testee.Verify(block);

            result.Should().BeTrue();
        }
    }
}