using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NeoSharp.Core.Network.Messages;
using NeoSharp.Core.Network.Serialization;

namespace NeoSharp.Core.Network.Tcp
{
    public class TcpPeer : IPeer, IDisposable
    {
        private readonly Socket _socket;
        private readonly IMessageSerializer _serializer;
        private readonly NetworkStream _stream;
        private readonly ILogger<TcpPeer> _logger;
        private int _disposed;

        public TcpPeer(Socket socket, IMessageSerializer serializer, ILogger<TcpPeer> logger)
        {
            _socket = socket ?? throw new ArgumentNullException(nameof(socket));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _stream = new NetworkStream(_socket);
        }

        public void Disconnect()
        {
            _logger.LogInformation("The peer was disconnected");
            Dispose();
        }

        public Task Send<TMessage>() where TMessage : Message, new()
        {
            return Send(new TMessage());
        }

        public async Task Send<TMessage>(TMessage message)
            where TMessage : Message
        {
            if (_disposed > 0) return;

            using (var source = new CancellationTokenSource(10000))
            {
                //Stream.WriteAsync doesn't support CancellationToken
                //see: https://stackoverflow.com/questions/20131434/cancel-networkstream-readasync-using-tcplistener
                source.Token.Register(Disconnect);

                try
                {
                    await _serializer.SerializeTo(message, _stream, source.Token);
                }
                catch (ObjectDisposedException) { }
                catch (Exception ex) when (ex is IOException || ex is OperationCanceledException)
                {
                    Disconnect();
                }
            }
        }

        public async Task<TMessage> Receive<TMessage>()
            where TMessage : Message, new()
        {
            if (_disposed > 0) return default(TMessage);

            using (var source = new CancellationTokenSource(10000))
            {
                //Stream.WriteAsync doesn't support CancellationToken
                //see: https://stackoverflow.com/questions/20131434/cancel-networkstream-readasync-using-tcplistener
                source.Token.Register(Disconnect);

                try
                {
                    return await _serializer.DeserializeFrom<TMessage>(_stream, source.Token);
                }
                catch (ArgumentException)
                {
                }
                catch (ObjectDisposedException)
                {
                }
                catch (Exception ex) when (ex is FormatException ||
                                           ex is IOException ||
                                           ex is OperationCanceledException)
                {
                    Disconnect();
                }
            }

            return default(TMessage);
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0) return;

            _socket.Shutdown(SocketShutdown.Both);
            _stream.Dispose();
            _socket.Dispose();
        }
    }
}
