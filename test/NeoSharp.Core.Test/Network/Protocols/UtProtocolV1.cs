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
    public class UtProtocolV1 : TestBase
    {
        [TestInitialize]
        public void WarmSerializer()
        {
            AutoMockContainer.Register<IBinaryConverter>(new BinaryConverter(typeof(VersionMessage).Assembly));
        }

        [TestMethod]
        public async Task Can_serialize_and_deserialize_messages()
        {
            // Arrange 
            var tcpProtocol = AutoMockContainer.Create<ProtocolV1>();
            var expectedVerAckMessage = new VerAckMessage();
            VerAckMessage actualVerAckMessage;

            // Act
            using (var memory = new MemoryStream())
            {
                await tcpProtocol.SendMessageAsync(memory, expectedVerAckMessage, CancellationToken.None);
                memory.Seek(0, SeekOrigin.Begin);
                actualVerAckMessage = (VerAckMessage)await tcpProtocol.ReceiveMessageAsync(memory, CancellationToken.None);
            }

            // Asset
            actualVerAckMessage.Should().NotBeNull();
            actualVerAckMessage.Command.Should().Be(expectedVerAckMessage.Command);
        }

        [TestMethod]
        public void Can_serialize_and_deserialize_messages_with_payload()
        {
            // Arrange 
            var tcpProtocol = AutoMockContainer.Create<ProtocolV1>();
            var expectedVersionMessage =
                new VersionMessage
                {
                    Payload =
                    {
                        Version = (uint)_rand.Next(0, int.MaxValue),
                        Services = (ulong)_rand.Next(0, int.MaxValue),
                        Timestamp = DateTime.UtcNow.ToTimestamp(),
                        Port = (ushort)_rand.Next(0, short.MaxValue),
                        Nonce = (uint)_rand.Next(0, int.MaxValue),
                        UserAgent = $"/NEO:{_rand.Next(1, 10)}.{_rand.Next(1, 100)}.{_rand.Next(1, 1000)}/",
                        CurrentBlockIndex = (uint)_rand.Next(0, int.MaxValue),
                        Relay = false
                    }
                };
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
            }

            // Asset
            actualVersionMessage.Should().NotBeNull();
            actualVersionMessage.Command.Should().Be(expectedVersionMessage.Command);
            actualVersionMessage.Payload.Should().NotBeNull();
            actualVersionMessage.Payload.Version.Should().Be(expectedVersionMessage.Payload.Version);
            actualVersionMessage.Payload.Services.Should().Be(expectedVersionMessage.Payload.Services);
            actualVersionMessage.Payload.Timestamp.Should().Be(expectedVersionMessage.Payload.Timestamp);
            actualVersionMessage.Payload.Port.Should().Be(expectedVersionMessage.Payload.Port);
            actualVersionMessage.Payload.Nonce.Should().Be(expectedVersionMessage.Payload.Nonce);
            actualVersionMessage.Payload.UserAgent.Should().Be(expectedVersionMessage.Payload.UserAgent);
            actualVersionMessage.Payload.CurrentBlockIndex.Should().Be(expectedVersionMessage.Payload.CurrentBlockIndex);
            actualVersionMessage.Payload.Relay.Should().Be(expectedVersionMessage.Payload.Relay);
        }
    }
}