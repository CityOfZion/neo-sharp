using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Messaging;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network.Protocols;

namespace NeoSharp.Core.Network.Tcp
{
    public class TcpPeer : IPeer, IDisposable
    {
        #region Constants

        private const int SocketOperationTimeout = 300_000;
        private const int MessageQueueCheckInterval = 100;

        #endregion

        #region Properties

        /// <summary>
        /// End Point
        /// </summary>
        public EndPoint EndPoint { get; }

        /// <summary>
        /// IpEndPoint
        /// </summary>
        public readonly IPEndPoint IPEndPoint;

        /// <summary>
        /// Is connected
        /// </summary>
        public bool IsConnected => _disposed == 0;

        /// <summary>
        /// BloomFilter
        /// </summary>
        public BloomFilter BloomFilter { get; set; }

        /// <summary>
        /// Version
        /// </summary>
        public VersionPayload Version { get; set; }

        /// <summary>
        /// IsReady
        /// </summary>
        public bool IsReady
        {
            get => IsConnected && _isReady;
            set => _isReady = value;
        }

        public DateTime ConnectionDate { get; }

        #endregion

        #region Variables

        private int _disposed;
        private bool _isReady;
        private ProtocolBase _protocol;

        private readonly Socket _socket;
        private readonly ProtocolSelector _protocolSelector;
        private readonly NetworkStream _stream;
        private readonly ILogger<TcpPeer> _logger;
        private readonly ConcurrentQueue<Message> _highPrioritySendMessageQueue = new ConcurrentQueue<Message>();
        private readonly ConcurrentQueue<Message> _lowPrioritySendMessageQueue = new ConcurrentQueue<Message>();
        private readonly CancellationTokenSource _messageSenderTokenSource = new CancellationTokenSource();

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="socket">Socket</param>
        /// <param name="protocolSelector">Protocol selector</param>
        /// <param name="logger">Logger</param>
        public TcpPeer(Socket socket, ProtocolSelector protocolSelector, ILogger<TcpPeer> logger)
        {
            _socket = socket ?? throw new ArgumentNullException(nameof(socket));
            _protocolSelector = protocolSelector ?? throw new ArgumentNullException(nameof(protocolSelector));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _stream = new NetworkStream(socket, true);
            _protocol = protocolSelector.DefaultProtocol;
            ConnectionDate = DateTime.UtcNow;

            // Extract address

            IPEndPoint = (IPEndPoint)(socket.IsBound ? socket.RemoteEndPoint : socket.LocalEndPoint);
            EndPoint = new EndPoint
            {
                Protocol = Protocol.Tcp,
                Host = IPEndPoint.Address.ToString(),
                Port = IPEndPoint.Port
            };

            SendMessages(_messageSenderTokenSource.Token);
        }

        /// <summary>
        /// Replace the current protocol
        /// </summary>
        /// <param name="version">Version</param>
        public bool ChangeProtocol(VersionPayload version)
        {
            if (version == null)
                return false;

            var protocol = _protocolSelector.GetProtocol(version);
            if (protocol == _protocol) return false;

            _protocol = protocol ?? throw new ArgumentException($"The protocol version \"{version}\" is not supported.", nameof(version));
            return true;
        }

        /// <summary>
        /// Disconnect
        /// </summary>
        public void Disconnect()
        {
            Dispose();
            _logger.LogInformation($"The peer {EndPoint.Host}:{EndPoint.Port} was disconnected");
            OnDisconnect?.Invoke(this, null);
        }

        public event EventHandler OnDisconnect;

        /// <summary>
        /// Free resources
        /// </summary>
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

        /// <summary>
        /// Send message Task
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
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

                    //TODO #422
                    if (message.Command != MessageCommand.consensus)	
                    {	
                        await InternalSend(message);	
                    }
                }
            },
            cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>Task</returns>
        public Task Send(Message message)
        {
            if (_protocol.IsHighPriorityMessage(message))
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

        /// <summary>
        /// Send default message
        /// </summary>
        /// <typeparam name="TMessage">Message</typeparam>
        /// <returns>Task</returns>
        public Task Send<TMessage>() where TMessage : Message, new()
        {
            return Send(new TMessage());
        }

        /// <summary>
        /// Receive message
        /// </summary>
        /// <returns>Message</returns>
        public async Task<Message> Receive()
        {
            if (!IsConnected) return null;

            using (var tokenSource = new CancellationTokenSource(SocketOperationTimeout))
            {
                tokenSource.Token.Register(Disconnect);

                try
                {
                    var msg = await _protocol.ReceiveMessageAsync(_stream, tokenSource.Token);
                    _logger.LogDebug($"Message Received: {msg.Command}");
                    return msg;
                }
                catch (Exception err)
                {
                    _logger.LogError(err, "Error while receive");
                    Disconnect();
                }
            }

            return null;
        }

        /// <summary>
        /// Receive message
        /// </summary>
        /// <typeparam name="TMessage">Type</typeparam>
        /// <returns>Message task</returns>
        public async Task<TMessage> Receive<TMessage>() where TMessage : Message, new()
        {
            if (!IsConnected) return null;
            var message = await Receive() as TMessage;
            _logger.LogDebug($"Message Received: {message.Command}");
            return message;

        }

        /// <summary>
        /// Real send message
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>Async task</returns>
        private async Task InternalSend(Message message)
        {
            if (!IsConnected) return;

            using (var tokenSource = new CancellationTokenSource(SocketOperationTimeout))
            {
                tokenSource.Token.Register(Disconnect);

                try
                {
                    _logger.LogDebug($"Message sent: {message.Command} to {EndPoint.Host}.");
                    await _protocol.SendMessageAsync(_stream, message, tokenSource.Token);
                }
                catch (Exception err)
                {
                    _logger.LogError(err, $"Error while send message {message.Command} to {EndPoint.Host}.");
                    Disconnect();
                }
            }
        }

        /// <summary>
        /// String representation
        /// </summary>
        public override string ToString()
        {
            return EndPoint.ToString();
        }
    }
}