using NeoSharp.Core.Blockchain;
using NeoSharp.Core.ExtensionMethods;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Helpers;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Messaging;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network.Security;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace NeoSharp.Core.Network
{
    public class Server : IServer, IDisposable
    {
        private const int DefaultReceiveTimeout = 1000;

        private readonly VersionPayload _version;
        private readonly INetworkAcl _acl;
        private readonly ILogger<Server> _logger;
        private readonly IAsyncDelayer _asyncDelayer;
        private readonly IBlockchain _blockchain;
        private readonly IPeerFactory _peerFactory;
        private readonly IPeerListener _peerListener;
        private readonly IMessageHandler<Message> _messageHandler;

        // if we successfully connect with a peer it is inserted into this list
        private readonly ConcurrentBag<IPeer> _connectedPeers;

        // if we can't connect to a peer it is inserted into this list
        // ReSharper disable once NotAccessedField.Local
        private readonly IList<IPEndPoint> _failedPeers;
        private readonly ushort _port;
        private readonly EndPoint[] _peerEndPoints;
        private CancellationTokenSource _messageListenerTokenSource;

        /// <summary>
        /// Get server version
        /// </summary>
        public VersionPayload Version
        {
            get
            {
                // update values

                _version.Timestamp = DateTime.UtcNow.ToTimestamp();
                _version.CurrentBlockIndex = _blockchain == null ? 0 : _blockchain.CurrentBlock == null ? 0 : _blockchain.CurrentBlock.Index;

                return _version;
            }
        }

        public Server(
            IBlockchain blockchain,
            NetworkConfig config,
            IPeerFactory peerFactory,
            IPeerListener peerListener,
            IMessageHandler<Message> messageHandler,
            ILogger<Server> logger,
            IAsyncDelayer asyncDelayer,
            NetworkAclFactory aclFactory)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            _blockchain = blockchain ?? throw new ArgumentNullException(nameof(blockchain));
            _peerFactory = peerFactory ?? throw new ArgumentNullException(nameof(peerFactory));
            _peerListener = peerListener ?? throw new ArgumentNullException(nameof(peerListener));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _asyncDelayer = asyncDelayer ?? throw new ArgumentNullException(nameof(asyncDelayer));
            if (aclFactory == null) throw new ArgumentNullException(nameof(aclFactory));

            _messageHandler = messageHandler;
            _acl = aclFactory.CreateNew();
            _acl?.Load(config.Acl);

            _peerListener.OnPeerConnected += PeerConnected;

            _connectedPeers = new ConcurrentBag<IPeer>();
            _failedPeers = new List<IPEndPoint>();

            // TODO: Change after port forwarding implementation
            _port = config.Port;
            _peerEndPoints = config.PeerEndPoints;

            _version = new VersionPayload
            {
                Version = 2,
                // TODO: What's it?
                // Services = NetworkAddressWithTime.NODE_NETWORK;
                Timestamp = DateTime.UtcNow.ToTimestamp(),
                Port = _port,
                Nonce = (uint)new Random(Environment.TickCount).Next(),
                UserAgent = $"/NEO:{Assembly.GetExecutingAssembly().GetName().Version.ToString(3)}/",
                CurrentBlockIndex = _blockchain == null ? 0 : _blockchain.CurrentBlock == null ? 0 : _blockchain.CurrentBlock.Index,
                Relay = true
            };
        }

        public IReadOnlyCollection<IPeer> ConnectedPeers => _connectedPeers;

        public void Start()
        {
            Stop();

            _messageListenerTokenSource = new CancellationTokenSource(DefaultReceiveTimeout);

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
                // TODO: no need to connect even to such peers
                if (_acl != null && !_acl.IsAllowed(peer))
                {
                    throw new UnauthorizedAccessException();
                }

                _connectedPeers.Add(peer);

                ListenForMessages(peer, _messageListenerTokenSource.Token);

                // initiate handshake
                peer.Send(new VersionMessage(_version));
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
            Parallel.ForEach(_peerEndPoints, async peerEndPoint =>
            {
                try
                {
                    var peer = await _peerFactory.ConnectTo(peerEndPoint);

                    PeerConnected(this, peer);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Something went wrong with {peerEndPoint}. Exception: {ex}");
                }
            });
        }

        private void DisconnectPeers()
        {
            foreach (var peer in ConnectedPeers)
            {
                peer.Disconnect();
            }

            _connectedPeers.Clear();
        }

        private void ListenForMessages(IPeer peer, CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(async () =>
            {
                while (peer.IsConnected)
                {
                    var message = await peer.Receive();

                    if (peer.IsReady == message.IsHandshakeMessage()) continue;

                    await _messageHandler.Handle(message, peer);

                    await _asyncDelayer.Delay(TimeSpan.FromMilliseconds(DefaultReceiveTimeout), cancellationToken);
                }
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }
    }
}