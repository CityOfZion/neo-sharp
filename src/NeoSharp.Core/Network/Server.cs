using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Messaging;
using NeoSharp.Core.Messaging.Messages;

namespace NeoSharp.Core.Network
{
    public class Server : IServer, IDisposable
    {
        private readonly NetworkConfig _config;
        private readonly IPeerFactory _peerFactory;
        private readonly IPeerListener _peerListener;
        private readonly IMessageHandler<Message> _messageHandler;
        private readonly ILogger<Server> _logger;

        private readonly ConcurrentBag<IPeer> _connectedPeers;                // if we successfully connect with a peer it is inserted into this list
        // ReSharper disable once NotAccessedField.Local
        private IList<IPEndPoint> _failedPeers;             // if we can't connect to a peer it is inserted into this list
        private readonly ushort _port;
        private readonly string _userAgent;
        private CancellationTokenSource _messageListenerTokenSource;

        private static readonly Type[] _handshakeMessageTypes = new[]
        {
            typeof(VersionMessage),
            typeof(VerAckMessage)
        };

        public Server(
            NetworkConfig config,
            IPeerFactory peerFactory,
            IPeerListener peerListener,
            IMessageHandler<Message> messageHandler, 
            ILogger<Server> logger)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _peerFactory = peerFactory ?? throw new ArgumentNullException(nameof(peerFactory));
            _peerListener = peerListener ?? throw new ArgumentNullException(nameof(peerListener));
            _messageHandler = messageHandler;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _peerListener.OnPeerConnected += PeerConnected;

            _connectedPeers = new ConcurrentBag<IPeer>();
            _failedPeers = new List<IPEndPoint>();

            // TODO: Change after port forwarding implementation
            _port = _config.Port;

            ProtocolVersion = 2;

            var r = new Random(Environment.TickCount);
            Nonce = (uint)r.Next();

            _userAgent = $"/NEO:{Assembly.GetExecutingAssembly().GetName().Version.ToString(3)}/";
        }

        public IReadOnlyCollection<IPeer> ConnectedPeers => _connectedPeers;

        public uint ProtocolVersion { get; }

        public uint Nonce { get; }

        public void Start()
        {
            Stop();

            _messageListenerTokenSource = new CancellationTokenSource(1000);

            // connect to peers
            ConnectToPeers();

            // listen for peers
            _peerListener.Start();
        }

        public void Stop()
        {
            _peerListener.Stop();

            // send disconnect to all current Peers
            DisconnectPeers();

            _messageListenerTokenSource?.Cancel();
        }

        public void Dispose()
        {
            Stop();
            _peerListener.OnPeerConnected -= PeerConnected;
        }

        private void PeerConnected(object sender, IPeer peer)
        {
            try
            {
                _connectedPeers.Add(peer);

                ListenForMessages(peer, _messageListenerTokenSource.Token);

                InitiateHandshaking(peer);
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Something went wrong with {peer}. Exception: {e}");
                peer.Disconnect();
            }
        }

        private void ConnectToPeers()
        {
            // TODO: check if localhot:port in seeding list
            foreach (var peerEndPoint in _config.PeerEndPoints)
            {
                _peerFactory
                    .ConnectTo(peerEndPoint)
                    .ContinueWith(t =>
                    {
                        if (t.IsCompletedSuccessfully)
                        {
                            PeerConnected(this, t.Result);
                        }
                        else
                        {
                            _logger.LogWarning($"Something went wrong with {peerEndPoint}. Exception: {t.Exception}");
                        }
                    });
            }
        }

        private void DisconnectPeers()
        {
            foreach (var peer in ConnectedPeers)
            {
                peer.Disconnect();
            }

            _connectedPeers.Clear();
        }

        private void InitiateHandshaking(IPeer peer)
        {
            var version = GetVersionMessage();

            peer.Send(version);
        }

        private VersionMessage GetVersionMessage()
        {
            var version = new VersionMessage();
            version.Payload.Version = ProtocolVersion;
            // TODO: What's it?
            // version.Payload.Services = NetworkAddressWithTime.NODE_NETWORK;
            version.Payload.Timestamp = DateTime.Now.ToTimestamp();
            version.Payload.Port = _port;
            version.Payload.Nonce = Nonce;
            version.Payload.UserAgent = _userAgent;
            // TODO: Inject blockchain and get height
            // version.Payload.StartHeight = Blockchain.Default?.Height ?? 0;
            version.Payload.Relay = true;

            return version;
        }

        private void ListenForMessages(IPeer peer, CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(async () =>
            {
                while (peer.IsConnected)
                {
                    var message = await peer.Receive();

                    if (!peer.IsReady && !IsHandshakeMessage(message)) continue;

                    await _messageHandler.Handle(message, peer);

                    await Task.Delay(1000, cancellationToken);
                }
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private static bool IsHandshakeMessage(Message m) => _handshakeMessageTypes.Contains(m.GetType());
    }
}
