using Microsoft.Extensions.Logging;
using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Messaging;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network.Tcp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace NeoSharp.Core.Network
{
    public class Server : IServer, IDisposable
    {
        private readonly INetworkACL _acl;
        private readonly ILogger<Server> _logger;
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
        private readonly string _userAgent;
        private CancellationTokenSource _messageListenerTokenSource;

        private static readonly Type[] _handshakeMessageTypes = {
            typeof(VersionMessage),
            typeof(VerAckMessage)
        };

        public Server(
            IBlockchain blockchain,
            NetworkConfig config,
            IPeerFactory peerFactory,
            IPeerListener peerListener,
            IMessageHandler<Message> messageHandler,
            ILogger<Server> logger,
            NetworkACLFactory aclFactory)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            _blockchain = blockchain ?? throw new ArgumentNullException(nameof(blockchain));
            _peerFactory = peerFactory ?? throw new ArgumentNullException(nameof(peerFactory));
            _peerListener = peerListener ?? throw new ArgumentNullException(nameof(peerListener));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            if (aclFactory == null) throw new ArgumentNullException(nameof(aclFactory));

            _messageHandler = messageHandler;
            _acl = aclFactory.CreateNew();
            _acl?.Load(config?.ACL);

            _peerListener.OnPeerConnected += PeerConnected;

            _connectedPeers = new ConcurrentBag<IPeer>();
            _failedPeers = new List<IPEndPoint>();

            // TODO: Change after port forwarding implementation
            _port = config.Port;

            ProtocolVersion = 2;

            var r = new Random(Environment.TickCount);
            Nonce = (uint)r.Next();

            _peerEndPoints = config.PeerEndPoints;

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

        private VersionMessage VersionMessage
        {
            get
            {
                // probably we can cache it
                var version = new VersionMessage
                {
                    Payload =
                    {
                        Version = ProtocolVersion,
                        // TODO: What's it?
                        // Services = NetworkAddressWithTime.NODE_NETWORK;
                        Timestamp = DateTime.UtcNow.ToTimestamp(),
                        Port = _port,
                        Nonce = Nonce,
                        UserAgent = _userAgent,
                        StartHeight = _blockchain.Height,
                        Relay = true
                    }
                };

                return version;
            }
        }

        private void PeerConnected(object sender, IPeer peer)
        {
            try
            {
                if (_acl != null)
                {
                    // Tcp

                    if (peer is TcpPeer tcp && tcp.IPAddress != null && !_acl.IsAllowed(tcp.IPAddress))
                    {
                        throw new UnauthorizedAccessException();
                    }
                    else
                    {
                        // Other protocols

                        if (peer.EndPoint != null && !_acl.IsAllowed(peer.EndPoint.Host))
                        {
                            throw new UnauthorizedAccessException();
                        }
                    }
                }

                _connectedPeers.Add(peer);

                ListenForMessages(peer, _messageListenerTokenSource.Token);

                // initiate handshake
                peer.Send(VersionMessage);
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
            foreach (var peerEndPoint in _peerEndPoints)
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
