using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NeoSharp.Core.Caching;
using NeoSharp.Core.Messaging;

namespace NeoSharp.Core.Network.Protocols
{
    public abstract class ProtocolBase
    {
        private const int MaxBufferSize = 1024;

        protected readonly ReflectionCache<MessageCommand> Cache = ReflectionCache<MessageCommand>.CreateFromEnum<MessageCommand>();

        /// <summary>
        /// Magic header protocol
        /// </summary>
        public abstract uint Version { get; }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="message">Message</param>
        /// <param name="cancellationToken">Cancel token</param>
        public abstract Task SendMessageAsync(Stream stream, Message message, CancellationToken cancellationToken);

        /// <summary>
        /// Receive message
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="cancellationToken">Cancel token</param>
        /// <returns>Return message or NULL</returns>
        public abstract Task<Message> ReceiveMessageAsync(Stream stream, CancellationToken cancellationToken);

        protected static async Task<byte[]> FillBufferAsync(
            Stream stream,
            int size,
            CancellationToken cancellationToken)
        {
            size = Math.Min(size, MaxBufferSize);
            var buffer = new byte[size];

            using (var memory = new MemoryStream())
            {
                while (size > 0)
                {
                    var count = Math.Min(size, buffer.Length);

                    count = await stream.ReadAsync(buffer, 0, count, cancellationToken);
                    if (count <= 0) throw new IOException();

                    memory.Write(buffer, 0, count);
                    size -= count;
                }

                return memory.ToArray();
            }
        }
    }
}