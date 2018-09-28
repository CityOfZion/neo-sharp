using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Blockchain.Processing;
using NeoSharp.Core.Models;
using NeoSharp.Core.Models.OperationManger;
using NeoSharp.Core.Types;
using NeoSharp.TestHelpers;

namespace NeoSharp.Core.Test.Network
{
    [TestClass]
    public class UtBlockVerifier : TestBase
    {
        
        [TestMethod]
        public void Verify_WithPrevBlockHeaderNotFound()
        {
            var testee = AutoMockContainer.Create<BlockVerifier>();

            var block = new Block
            {
                PreviousBlockHash = UInt256.Parse("7ee8170d86de43d6c105699273f9b82d077180e5e0f8e4d942f43d7804cc54d3")
            };
            
            var blockchainMock = AutoMockContainer.GetMock<IBlockchain>();
            blockchainMock.Setup(b => b.GetBlockHeader(block.PreviousBlockHash)).ReturnsAsync(() => null);
            
            var result = testee.Verify(block);
            
            result.Should().BeFalse();
        }
        
        [TestMethod]
        public void Verify_WithPrevBlockHeaderIndexNotThePrevious()
        {
            var testee = AutoMockContainer.Create<BlockVerifier>();

            var block = new Block
            {
                PreviousBlockHash = UInt256.Parse("7ee8170d86de43d6c105699273f9b82d077180e5e0f8e4d942f43d7804cc54d3"),
                Index = 3
            };

            var prevBlockHeader = new BlockHeader
            {
                Index = 1
            };
            
            var blockchainMock = AutoMockContainer.GetMock<IBlockchain>();
            blockchainMock.Setup(b => b.GetBlockHeader(block.PreviousBlockHash)).ReturnsAsync(() => prevBlockHeader);
            
            var result = testee.Verify(block);
            
            result.Should().BeFalse();
        }
        
        [TestMethod]
        public void Verify_WithPrevBlockHeaderGreaterTimestamp()
        {
            var testee = AutoMockContainer.Create<BlockVerifier>();

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
            
            var blockchainMock = AutoMockContainer.GetMock<IBlockchain>();
            blockchainMock.Setup(b => b.GetBlockHeader(block.PreviousBlockHash)).ReturnsAsync(() => prevBlockHeader);
            
            var result = testee.Verify(block);
            
            result.Should().BeFalse();
        }
        
        [TestMethod]
        public void Verify_WithPrevBlockHeaderVerifyWitnessFail()
        {
            var testee = AutoMockContainer.Create<BlockVerifier>();

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
            
            var blockchainMock = AutoMockContainer.GetMock<IBlockchain>();
            blockchainMock.Setup(b => b.GetBlockHeader(block.PreviousBlockHash)).ReturnsAsync(() => prevBlockHeader);
            
            var witnessOMMock = AutoMockContainer.GetMock<IWitnessOperationsManager>();
            witnessOMMock.Setup(b => b.Verify(block.Witness)).Returns(() => false);
            
            var result = testee.Verify(block);
            
            result.Should().BeFalse();
        }
        
        [TestMethod]
        public void Verify_Success()
        {
            var testee = AutoMockContainer.Create<BlockVerifier>();

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
            
            var blockchainMock = AutoMockContainer.GetMock<IBlockchain>();
            blockchainMock.Setup(b => b.GetBlockHeader(block.PreviousBlockHash)).ReturnsAsync(() => prevBlockHeader);
            
            var witnessOMMock = AutoMockContainer.GetMock<IWitnessOperationsManager>();
            witnessOMMock.Setup(b => b.Verify(block.Witness)).Returns(() => true);
            
            var result = testee.Verify(block);

            result.Should().BeTrue();
        }
    }
}