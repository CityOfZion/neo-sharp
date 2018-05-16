using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Messaging;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network.Protocols;
using NeoSharp.TestHelpers;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NeoSharp.Core.Test.Network.Protocols
{
    [TestClass]
    public class UtProtocolV2 : TestBase
    {
        [TestInitialize]
        public void WarmSerializer()
        {
            AutoMockContainer.Register<IBinaryConverter>(new BinaryConverter(typeof(VersionMessage).Assembly));
        }

        [TestMethod]
        public void Can_serialize_and_deserialize_messages()
        {
            // Arrange 
            var tcpProtocol = AutoMockContainer.Create<ProtocolV2>();
            var expectedVerAckMessage = new VerAckMessage();
            VerAckMessage actualVerAckMessage;

            // Act
            using (var memory = new MemoryStream())
            {
                Task a = tcpProtocol.SendMessageAsync(memory, expectedVerAckMessage, CancellationToken.None);
                a.Wait();
                memory.Seek(0, SeekOrigin.Begin);
                Task<Message> b = tcpProtocol.ReceiveMessageAsync(memory, CancellationToken.None);
                b.Wait();
                actualVerAckMessage = (VerAckMessage)b.Result;
            }

            // Asset
            actualVerAckMessage.Should().NotBeNull();
            actualVerAckMessage.Command.Should().Be(expectedVerAckMessage.Command);
        }

        [TestMethod]
        public void Can_serialize_and_deserialize_messages_with_payload_uncompressed()
        {
            Assert.IsFalse(Can_serialize_and_deserialize_messages_with_payload(0));
        }

        [TestMethod]
        public void Can_serialize_and_deserialize_messages_with_payload_compressed()
        {
            Assert.IsTrue(Can_serialize_and_deserialize_messages_with_payload(200));
        }

        public bool Can_serialize_and_deserialize_messages_with_payload(int length)
        {
            // Arrange
            var isCompressed = false;
            var tcpProtocol = AutoMockContainer.Create<ProtocolV2>();
            var expectedVersionMessage = new VersionMessage();
            expectedVersionMessage.Payload.Version = (uint)_rand.Next(0, int.MaxValue);
            expectedVersionMessage.Payload.Services = (ulong)_rand.Next(0, int.MaxValue);
            expectedVersionMessage.Payload.Timestamp = DateTime.UtcNow.ToTimestamp();
            expectedVersionMessage.Payload.Port = (ushort)_rand.Next(0, short.MaxValue);
            expectedVersionMessage.Payload.Nonce = (uint)_rand.Next(0, int.MaxValue);
            expectedVersionMessage.Payload.UserAgent = $"/NEO:{_rand.Next(1, 10)}.{_rand.Next(1, 100)}.{_rand.Next(1, 1000)}/" + ("0".PadLeft(length, '0'));
            expectedVersionMessage.Payload.StartHeight = (uint)_rand.Next(0, int.MaxValue);
            expectedVersionMessage.Payload.Relay = false;
            VersionMessage actualVersionMessage;

            // Act
            using (var memory = new MemoryStream())
            {
                Task a = tcpProtocol.SendMessageAsync(memory, expectedVersionMessage, CancellationToken.None);
                a.Wait();
                memory.Seek(0, SeekOrigin.Begin);
                Task<Message> b = tcpProtocol.ReceiveMessageAsync(memory, CancellationToken.None);
                b.Wait();
                actualVersionMessage = (VersionMessage)b.Result;
                isCompressed = b.Result.Flags.HasFlag(MessageFlags.Compressed);
            }

            // Asset
            actualVersionMessage.Should().NotBeNull();
            actualVersionMessage.Flags.Should().Be(expectedVersionMessage.Flags);
            actualVersionMessage.Command.Should().Be(expectedVersionMessage.Command);
            actualVersionMessage.Payload.Should().NotBeNull();
            actualVersionMessage.Payload.Version.Should().Be(expectedVersionMessage.Payload.Version);
            actualVersionMessage.Payload.Services.Should().Be(expectedVersionMessage.Payload.Services);
            actualVersionMessage.Payload.Timestamp.Should().Be(expectedVersionMessage.Payload.Timestamp);
            actualVersionMessage.Payload.Port.Should().Be(expectedVersionMessage.Payload.Port);
            actualVersionMessage.Payload.Nonce.Should().Be(expectedVersionMessage.Payload.Nonce);
            actualVersionMessage.Payload.UserAgent.Should().Be(expectedVersionMessage.Payload.UserAgent);
            actualVersionMessage.Payload.StartHeight.Should().Be(expectedVersionMessage.Payload.StartHeight);
            actualVersionMessage.Payload.Relay.Should().Be(expectedVersionMessage.Payload.Relay);

            return isCompressed;
        }
    }
}