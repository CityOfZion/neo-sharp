using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NeoSharp.Core.Messaging;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network.Protocols;

namespace NeoSharp.Core.Network.Tcp
{
    public class TcpPeer : IPeer, IDisposable
    {
        private const int SocketOperationTimeout = 300_000;
        private const int MessageQueueCheckInterval = 100;

        private static readonly Type[] _highPrioritySendMessageTypes =
        {
            typeof(VersionMessage),
            typeof(VerAckMessage)
        };

        private readonly Socket _socket;
        private readonly ProtocolSelector _protocolSelector;
        private readonly NetworkStream _stream;
        private ProtocolBase _protocol;
        private readonly ILogger<TcpPeer> _logger;
        private int _disposed;
        private bool _isReady;
        private readonly ConcurrentQueue<Message> _highPrioritySendMessageQueue = new ConcurrentQueue<Message>();
        private readonly ConcurrentQueue<Message> _lowPrioritySendMessageQueue = new ConcurrentQueue<Message>();
        private readonly CancellationTokenSource _messageSenderTokenSource = new CancellationTokenSource();

        public TcpPeer(Socket socket, ProtocolSelector protocolSelector, ILogger<TcpPeer> logger)
        {
            _socket = socket ?? throw new ArgumentNullException(nameof(socket));
            _protocolSelector = protocolSelector ?? throw new ArgumentNullException(nameof(protocolSelector));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _stream = new NetworkStream(socket, true);
            _protocol = protocolSelector.DefaultProtocol;

            SendMessages(_messageSenderTokenSource.Token);
        }

        private void SendMessages(CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(async () =>
            {
                while (IsConnected)
                {
                    if (!_highPrioritySendMessageQueue.TryDequeue(out var message))
                    {
                        if (!_lowPrioritySendMessageQueue.TryDequeue(out message))
                        {
                            await Task.Delay(MessageQueueCheckInterval, cancellationToken);
                            continue;
                        }
                    }

                    await InternalSend(message);
                }
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public bool IsConnected => _disposed == 0;

        public bool IsReady
        {
            get => IsConnected && _isReady;
            set => _isReady = value;
        }

        public void DowngradeProtocol(uint version)
        {
            if (version > _protocol.Version)
                throw new ArgumentException($"The protocol version must to be lower than \"{_protocol.Version}\"",
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
                _messageSenderTokenSource.Cancel();
                _socket.Shutdown(SocketShutdown.Both);
                _stream.Dispose();
                _socket.Dispose();
            }
            catch
            {
                // ignored
            }
        }

        public Task Send(Message message)
        {
            if (IsHighPriorityMessage(message))
            {
                _highPrioritySendMessageQueue.Enqueue(message);
            }
            else
            {
                _lowPrioritySendMessageQueue.Enqueue(message);
            }

            // we don't wait until the message will be picked up from the queue and sent over the network
            return Task.CompletedTask;
        }

        public Task Send<TMessage>() where TMessage : Message, new()
        {
            return Send(new TMessage());
        }

        public async Task<Message> Receive()
        {
            if (!IsConnected) return null;

            using (var tokenSource = new CancellationTokenSource(SocketOperationTimeout))
            {
                tokenSource.Token.Register(Disconnect);

                try
                {
                    return await _protocol.ReceiveMessageAsync(_stream, tokenSource.Token);
                }
                catch (Exception)
                {
                    Disconnect();
                }
            }

            return null;
        }

        public async Task<TMessage> Receive<TMessage>() where TMessage : Message, new()
        {
            if (!IsConnected) return null;

            return await Receive() as TMessage;
        }

        private static bool IsHighPriorityMessage(Message m) => _highPrioritySendMessageTypes.Contains(m.GetType());

        private async Task InternalSend(Message message)
        {
            if (!IsConnected) return;

            using (var tokenSource = new CancellationTokenSource(SocketOperationTimeout))
            {
                tokenSource.Token.Register(Disconnect);

                try
                {
                    await _protocol.SendMessageAsync(_stream, message, tokenSource.Token);
                }
                catch (Exception)
                {
                    Disconnect();
                }
            }
        }
    }
}