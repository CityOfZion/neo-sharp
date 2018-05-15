using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Caching;
using NeoSharp.Core.Messaging.Handlers;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Models;
using NeoSharp.Core.Network;
using NeoSharp.Core.Types;
using NeoSharp.TestHelpers;

namespace NeoSharp.Core.Test.Messaging.Handlers
{
    [TestClass]
    public class UtVerackMessageHandler : TestBase
    {
        class NullBlockchain : IBlockchain
        {
            public UInt256 CurrentBlockHash { get; set; }
            public UInt256 CurrentBlockHeaderHash { get; set; }
            public uint BlockHeaderHeight { get; set; }
            public uint Height { get; set; }
            public bool AddBlock(Block block)
            {
                throw new NotImplementedException();
            }

            public bool ContainsBlock(UInt256 hash)
            {
                throw new NotImplementedException();
            }

            public bool ContainsTransaction(UInt256 hash)
            {
                throw new NotImplementedException();
            }

            public bool ContainsUnspent(CoinReference input)
            {
                throw new NotImplementedException();
            }

            public bool ContainsUnspent(UInt256 hash, ushort index)
            {
                throw new NotImplementedException();
            }

            public MetaDataCache<T> GetMetaData<T>() where T : class, ISerializable, new()
            {
                throw new NotImplementedException();
            }

            public Block GetBlock(uint height)
            {
                throw new NotImplementedException();
            }

            public Block GetBlock(UInt256 hash)
            {
                throw new NotImplementedException();
            }

            public UInt256 GetBlockHash(uint height)
            {
                throw new NotImplementedException();
            }

            public BlockHeader GetBlockHeader(uint height)
            {
                throw new NotImplementedException();
            }

            public BlockHeader GetBlockHeader(UInt256 hash)
            {
                throw new NotImplementedException();
            }

            public ECPoint[] GetValidators()
            {
                throw new NotImplementedException();
            }

            public IEnumerable<ECPoint> GetValidators(IEnumerable<Transaction> others)
            {
                throw new NotImplementedException();
            }

            public Block GetNextBlock(UInt256 hash)
            {
                throw new NotImplementedException();
            }

            public UInt256 GetNextBlockHash(UInt256 hash)
            {
                throw new NotImplementedException();
            }

            public long GetSysFeeAmount(uint height)
            {
                throw new NotImplementedException();
            }

            public long GetSysFeeAmount(UInt256 hash)
            {
                throw new NotImplementedException();
            }

            public Transaction GetTransaction(UInt256 hash)
            {
                throw new NotImplementedException();
            }

            public Transaction GetTransaction(UInt256 hash, out int height)
            {
                throw new NotImplementedException();
            }

            public TransactionOutput GetUnspent(UInt256 hash, ushort index)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<TransactionOutput> GetUnspent(UInt256 hash)
            {
                throw new NotImplementedException();
            }

            public bool IsDoubleSpend(Transaction tx)
            {
                throw new NotImplementedException();
            }

            public void AddBlockHeaders(IEnumerable<BlockHeader> blockHeaders)
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public  async Task Can_send_get_block_headers_message_if_peer_block_header_height_is_different()
        {
            // Arrange
            var blockchain = new NullBlockchain();

            AutoMockContainer.Register<IBlockchain>(blockchain);

            var verAckMessage = new VerAckMessage();
            var version = new VersionPayload();
            var peerMock = AutoMockContainer.GetMock<IPeer>();

            peerMock.SetupProperty(x => x.Version, version);

            blockchain.BlockHeaderHeight = 1;
            version.StartHeight = 2;

            var messageHandler = AutoMockContainer.Get<VerAckMessageHandler>();

            // Act
            await messageHandler.Handle(verAckMessage, peerMock.Object);

            // Assert
            peerMock.Verify(x => x.Send(It.IsAny<GetBlockHeadersMessage>()), Times.Once);
        }

        [TestMethod]
        public async Task Can_activate_peer_on_verack_receiving()
        {
            // Arrange
            var blockchain = new NullBlockchain();

            AutoMockContainer.Register<IBlockchain>(blockchain);

            var verAckMessage = new VerAckMessage();
            var version = new VersionPayload();
            var peerMock = AutoMockContainer.GetMock<IPeer>();

            peerMock.SetupProperty(x => x.IsReady, false);
            peerMock.SetupProperty(x => x.Version, version);

            blockchain.BlockHeaderHeight = 1;
            version.StartHeight = 1;

            var messageHandler = AutoMockContainer.Get<VerAckMessageHandler>();

            // Act
            await messageHandler.Handle(verAckMessage, peerMock.Object);

            // Assert
            peerMock.Object.IsReady.Should().BeTrue();
        }
    }
}