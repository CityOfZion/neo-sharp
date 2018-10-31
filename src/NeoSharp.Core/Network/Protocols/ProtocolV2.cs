using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Exceptions;
using NeoSharp.Core.Messaging;
using NeoSharp.Types.ExtensionMethods;

namespace NeoSharp.Core.Network.Protocols
{
    public class ProtocolV2 : ProtocolBase
    {
        #region Properties

        /// <summary>
        /// Protocol version
        /// </summary>
        public override uint Version => 2;

        #endregion

        #region Variables

        private readonly uint _magic;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config">Configuration</param>
        public ProtocolV2(NetworkConfig config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            _magic = config.Magic;
        }

        public override bool IsHighPriorityMessage(Message m)
        {
            return m.Flags.HasFlag(MessageFlags.Urgent) || base.IsHighPriorityMessage(m);
        }

        public override async Task SendMessageAsync(Stream stream, Message message,
            CancellationToken cancellationToken)
        {
            using (var memory = new MemoryStream())
            using (var writer = new BinaryWriter(memory, Encoding.UTF8))
            {
                // TODO #366: Remove this magic in V2, only for handshake
                writer.Write(_magic);
                writer.Write((byte)message.Command);

                if (message is ICarryPayload messageWithPayload)
                {
                    message.Flags |= MessageFlags.WithPayload;
                    byte[] payloadBuffer = BinarySerializer.Default.Serialize(messageWithPayload.Payload);

                    if (payloadBuffer.Length > 100)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            using (GZipStream gzip = new GZipStream(ms, CompressionLevel.Fastest, true))
                                gzip.Write(payloadBuffer, 0, payloadBuffer.Length);

                            if (payloadBuffer.Length > ms.Length)
                            {
                                payloadBuffer = ms.ToArray();
                                message.Flags |= MessageFlags.Compressed;
                            }
                        }
                    }

                    writer.Write((byte)message.Flags);
                    writer.Write((uint)payloadBuffer.Length);
                    writer.Write(payloadBuffer);
                }
                else
                {
                    message.Flags &= ~MessageFlags.WithPayload;
                    writer.Write((byte)message.Flags);
                }

                writer.Flush();

                var buffer = memory.ToArray();
                await stream.WriteAsync(buffer, 0, buffer.Length, cancellationToken);
            }
        }

        public override async Task<Message> ReceiveMessageAsync(Stream stream, CancellationToken cancellationToken)
        {
            var buffer = await FillBufferAsync(stream, 6, cancellationToken);

            // TODO #366: Remove this magic in V2, only for handshake
            if (buffer.ToInt32(0) != _magic)
            {
                throw new InvalidMessageException();
            }

            var command = (MessageCommand)buffer[4];

            if (!Cache.TryGetValue(command, out var type))
            {
                throw (new InvalidMessageException("Message command not found"));
            }

            Message message;
            var flags = (MessageFlags)buffer[5];

            if (flags.HasFlag(MessageFlags.WithPayload) && typeof(ICarryPayload).IsAssignableFrom(type))
            {
                buffer = await FillBufferAsync(stream, 4, cancellationToken);

                var payloadLength = buffer.ToInt32(0);
                if (payloadLength > Message.PayloadMaxSize)
                {
                    throw new InvalidMessageException();
                }

                var payloadBuffer = payloadLength > 0
                    ? await FillBufferAsync(stream, (int)payloadLength, cancellationToken)
                    : new byte[0];

                if (payloadLength == 0)
                {
                    throw new InvalidMessageException();
                }

                var payloadType = type.BaseType.GenericTypeArguments[0];

                if (flags.HasFlag(MessageFlags.Compressed))
                {
                    using (var ms = new MemoryStream(payloadBuffer))
                    using (var gzip = new GZipStream(ms, CompressionMode.Decompress))
                    {
                        var payload = BinarySerializer.Default.Deserialize(gzip, payloadType);
                        message = (Message)Activator.CreateInstance(type, payload);
                    }
                }
                else
                {
                    var payload = BinarySerializer.Default.Deserialize(payloadBuffer, payloadType);
                    message = (Message)Activator.CreateInstance(type, payload);
                }
            }
            else
            {
                message = (Message)Activator.CreateInstance(type);
            }

            message.Flags = flags;
            return message;
        }
    }
}