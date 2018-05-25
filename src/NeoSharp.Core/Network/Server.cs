using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Messaging;
using NeoSharp.Core.Messaging.Messages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NeoSharp.Core.ExtensionMethods;

namespace NeoSharp.Core.Network
{
    public class Server : IServer, IDisposable
    {
        private readonly INetworkACL _acl;
        private readonly Logging.ILoggerProvider<Server> _logger;
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

        public Server(
            IBlockchain blockchain,
            NetworkConfig config,
            IPeerFactory peerFactory,
            IPeerListener peerListener,
            IMessageHandler<Message> messageHandler,
            Logging.ILoggerProvider<Server> logger,
            NetworkACLFactory aclFactory)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            this._blockchain = blockchain ?? throw new ArgumentNullException(nameof(blockchain));
            this._peerFactory = peerFactory ?? throw new ArgumentNullException(nameof(peerFactory));
            this._peerListener = peerListener ?? throw new ArgumentNullException(nameof(peerListener));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            if (aclFactory == null) throw new ArgumentNullException(nameof(aclFactory));

            this._messageHandler = messageHandler;
            this._acl = aclFactory.CreateNew();
            this._acl?.Load(config?.ACL);

            this._peerListener.OnPeerConnected += PeerConnected;

            this._connectedPeers = new ConcurrentBag<IPeer>();
            this._failedPeers = new List<IPEndPoint>();

            // TODO: Change after port forwarding implementation
            this._port = config.Port;

            ProtocolVersion = 2;

            var r = new Random(Environment.TickCount);
            Nonce = (uint) r.Next();

            this._peerEndPoints = config.PeerEndPoints;

            this._userAgent = $"/NEO:{Assembly.GetExecutingAssembly().GetName().Version.ToString(3)}/";
        }

        public IReadOnlyCollection<IPeer> ConnectedPeers => this._connectedPeers;

        public uint ProtocolVersion { get; }

        public uint Nonce { get; }

        public void Start()
        {
            Stop();

            this._messageListenerTokenSource = new CancellationTokenSource(1000);

            // connect to peers
            this.ConnectToPeers();

            // listen for peers
            this._peerListener.Start();
        }

        public void Stop()
        {
            this._peerListener.Stop();

            // send disconnect to all current Peers
            this.DisconnectPeers();

            this._messageListenerTokenSource?.Cancel();
        }

        public void Dispose()
        {
            this.Stop();
            this._peerListener.OnPeerConnected -= PeerConnected;
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
                        Port = this._port,
                        Nonce = Nonce,
                        UserAgent = _userAgent,
                        CurrentBlockIndex = this._blockchain.CurrentBlock.Index,
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
                if (_acl != null && !_acl.IsAllowed(peer))
                {
                    throw new UnauthorizedAccessException();
                }

                this._connectedPeers.Add(peer);

                this.ListenForMessages(peer, this._messageListenerTokenSource.Token);

                // initiate handshake
                peer.Send(this.VersionMessage);
            }
            catch (Exception e)
            {
                this._logger.LogWarning($"Something went wrong with {peer}. Exception: {e}");
                peer.Disconnect();
            }
        }

        private void ConnectToPeers()
        {
            // TODO: check if localhot:port in seeding list
            foreach (var peerEndPoint in _peerEndPoints)
            {
                this._peerFactory
                    .ConnectTo(peerEndPoint)
                    .ContinueWith(t =>
                    {
                        if (t.IsCompletedSuccessfully)
                        {
                            this.PeerConnected(this, t.Result);
                        }
                        else
                        {
                            var exceptionMessage = $"Something went wrong with {peerEndPoint}. Exception: {t.Exception}";
                            this._logger.LogWarning(exceptionMessage);
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

            this._connectedPeers.Clear();
        }

        private void ListenForMessages(IPeer peer, CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(async () =>
            {
                while (peer.IsConnected)
                {
                    var message = await peer.Receive();

                    if (!peer.IsReady && message.IsNotHandshakeMessage()) continue;

                    await this._messageHandler.Handle(message, peer);

                    await Task.Delay(1000, cancellationToken);
                }
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }
    }
}
