using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Core.Blockchain.Repositories;
using NeoSharp.Core.Models;
using NeoSharp.Core.Models.OperationManager;
using NeoSharp.TestHelpers;
using NeoSharp.Types;

namespace NeoSharp.Core.Test.Models
{
    [TestClass]
    public class UtBlockVerifier : TestBase
    {
        
        [TestMethod]
        public async Task Verify_WithPrevBlockHeaderNotFound()
        {
            var testee = AutoMockContainer.Create<BlockOperationManager>();

            var block = new Block
            {
                PreviousBlockHash = UInt256.Parse("7ee8170d86de43d6c105699273f9b82d077180e5e0f8e4d942f43d7804cc54d3")
            };
            
            AutoMockContainer
                .GetMock<IBlockRepository>()
                .Setup(b => b.GetBlockHeader(block.PreviousBlockHash))
                .ReturnsAsync(() => null);
            
            var result = await testee.Verify(block);
            
            result.Should().BeFalse();
        }
        
        [TestMethod]
        public async Task Verify_WithPrevBlockHeaderIndexNotThePrevious()
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
            
            AutoMockContainer
                .GetMock<IBlockRepository>()
                .Setup(b => b.GetBlockHeader(block.PreviousBlockHash))
                .ReturnsAsync(() => prevBlockHeader);
            
            var result = await testee.Verify(block);
            
            result.Should().BeFalse();
        }
        
        [TestMethod]
        public async Task Verify_WithPrevBlockHeaderGreaterTimestamp()
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

            AutoMockContainer
                .GetMock<IBlockRepository>()
                .Setup(b => b.GetBlockHeader(block.PreviousBlockHash))
                .ReturnsAsync(() => prevBlockHeader);

            var result = await testee.Verify(block);
            
            result.Should().BeFalse();
        }
        
        [TestMethod]
        public async Task Verify_WithPrevBlockHeaderVerifyWitnessFail()
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

            AutoMockContainer
                .GetMock<IBlockRepository>()
                .Setup(b => b.GetBlockHeader(block.PreviousBlockHash))
                .ReturnsAsync(() => prevBlockHeader);

            AutoMockContainer
                .GetMock<IWitnessOperationsManager>()
                .Setup(b => b.Verify(block.Witness))
                .Returns(() => Task.FromResult(false));
            
            var result = await testee.Verify(block);
            
            result.Should().BeFalse();
        }
        
        [TestMethod]
        public async Task Verify_Success()
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

            AutoMockContainer
                .GetMock<IBlockRepository>()
                .Setup(b => b.GetBlockHeader(block.PreviousBlockHash))
                .ReturnsAsync(() => prevBlockHeader);

            AutoMockContainer
                .GetMock<IWitnessOperationsManager>()
                .Setup(b => b.Verify(block.Witness))
                .Returns(() => Task.FromResult(true));
            
            var result = await testee.Verify(block);

            result.Should().BeTrue();
        }
    }
}