using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace NeoSharp.Core.Network.Tcp
{
    public abstract class ITcpProtocol
    {
        #region Constants

        protected const int MaxBufferSize = 1024;
        protected const int TimeOut = 30_000;

        #endregion

        /// <summary>
        /// Magic header protocol
        /// </summary>
        public virtual uint MagicHeader { get; }

#pragma warning disable CS1998

        /// <summary>
        /// Get message
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="cancellationToken">Cancel token</param>
        /// <returns>Return message or NULL</returns>
        public virtual async Task<TcpMessage> GetMessageAsync(NetworkStream stream, CancellationTokenSource cancellationToken)
        {
            throw new NotImplementedException();
        }
#pragma warning restore

        protected static async Task<byte[]> FillBufferAsync(NetworkStream stream, int buffer_size, CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[buffer_size < MaxBufferSize ? buffer_size : MaxBufferSize];
            using (MemoryStream ms = new MemoryStream())
            {
                while (buffer_size > 0)
                {
                    int count = buffer_size < MaxBufferSize ? buffer_size : MaxBufferSize;
                    count = await stream.ReadAsync(buffer, 0, count, cancellationToken);
                    if (count <= 0) throw new IOException();
                    ms.Write(buffer, 0, count);
                    buffer_size -= count;
                }
                return ms.ToArray();
            }
        }
    }
}