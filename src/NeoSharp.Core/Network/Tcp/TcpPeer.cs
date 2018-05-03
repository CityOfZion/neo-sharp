using Microsoft.Extensions.Logging;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NeoSharp.Core.Messaging;

namespace NeoSharp.Core.Network.Tcp
{
    public class TcpPeer : IPeer, IDisposable
    {
        private const int SocketOperationTimeout = 30_000;

        private readonly Socket _socket;
        private readonly TcpProtocolSelector _protocolSelector;
        private TcpProtocolBase _protocol;
        private readonly NetworkStream _stream;
        private readonly ILogger<TcpPeer> _logger;
        private int _disposed;
        private bool _isReady;

        public TcpPeer(Socket socket, TcpProtocolSelector protocolSelector, ILogger<TcpPeer> logger)
        {
            _socket = socket ?? throw new ArgumentNullException(nameof(socket));
            _protocolSelector = protocolSelector ?? throw new ArgumentNullException(nameof(protocolSelector));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _stream = new NetworkStream(socket, true);
            _protocol = protocolSelector.DefaultProtocol;
        }

        public bool IsConnected => _disposed == 0;

        public bool IsReady
        {
            get => IsConnected && _isReady;
            set => _isReady = value;
        }

        public void DowngradeProtocol(uint version)
        {
            if (version > _protocol.MagicHeader)
                throw new ArgumentException($"The protocol version must to be lower than \"{_protocol.MagicHeader}\"",
                    nameof(version));

            var protocol = _protocolSelector.GetProtocol(version);

            _protocol = protocol ?? throw new NotSupportedException("The protocol version is not supported.");
        }

        public void Disconnect()
        {
            Dispose();
            _logger.LogInformation("The peer was disconnected");
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0) return;

            try
            {
                _socket.Shutdown(SocketShutdown.Both);
                _stream.Dispose();
                _socket.Dispose();
            }
            catch
            {
                // ignored
            }
        }

        public Task Send<TMessage>() where TMessage : Message, new()
        {
            return Send(new TMessage());
        }

        public async Task Send<TMessage>(TMessage message) where TMessage : Message
        {
            if (_disposed > 0) return;

            using (var source = new CancellationTokenSource(SocketOperationTimeout))
            {
                source.Token.Register(Disconnect);

                try
                {
                    await _protocol.SendMessageAsync(_stream, message, source.Token);
                }
                catch (Exception)
                {
                    Disconnect();
                }
            }
        }

        public async Task<Message> Receive()
        {
            if (_disposed > 0) return null;

            using (var source = new CancellationTokenSource(SocketOperationTimeout))
            {
                source.Token.Register(Disconnect);

                try
                {
                    return await _protocol.ReceiveMessageAsync(_stream, source.Token);
                }
                catch (Exception)
                {
                    Disconnect();
                }
            }

            return null;
        }

        public Task<TMessage> Receive<TMessage>() where TMessage : Message, new()
        {
            if (_disposed > 0) return null;

            // TODO: This does not work
            return new Task<TMessage>(() =>
            {
                var msg = Receive();
                msg.Wait();

                return new TMessage();
            });
        }
    }
}