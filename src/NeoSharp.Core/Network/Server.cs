using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Messaging;
using NeoSharp.Core.Network.Security;

namespace NeoSharp.Core.Network
{
    public class Server : IServer, IBroadcaster, IDisposable
    {
        #region Private Fields

        private readonly ILogger<Server> _logger;
        private readonly IPeerMessageListener _peerMessageListener;
        private readonly IServerContext _serverContext;
        private readonly IEnumerable<IServerProcess> _serverProcesses;
        private readonly IPeerFactory _peerFactory;
        private readonly IPeerListener _peerListener;
        private readonly NetworkAcl _acl;

        private readonly EndPoint[] _peerEndPoints;
        private CancellationTokenSource _messageListenerTokenSource;
        private bool _isRunning;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config">Network configuration</param>
        /// <param name="aclLoader">ACL loader to define access</param>
        /// <param name="peerFactory">Factory to create peers from endpoints</param>
        /// <param name="peerListener">Listener to accept peer connections</param>
        /// <param name="peerMessageListener">PeerMessageListener</param>
        /// <param name="serverContext">Server context</param>
        /// <param name="serverProcesses">Server processes</param>
        /// <param name="logger">Logger</param>
        public Server(
            NetworkConfig config,
            INetworkAclLoader aclLoader,
            IPeerFactory peerFactory,
            IPeerListener peerListener,
            IPeerMessageListener peerMessageListener,
            IServerContext serverContext,
            IEnumerable<IServerProcess> serverProcesses,
            ILogger<Server> logger)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            _peerFactory = peerFactory ?? throw new ArgumentNullException(nameof(peerFactory));
            _peerListener = peerListener ?? throw new ArgumentNullException(nameof(peerListener));

            if (aclLoader == null) throw new ArgumentNullException(nameof(aclLoader));
            _acl = aclLoader.Load(config.AclConfig) ?? NetworkAcl.Default;

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _peerMessageListener = peerMessageListener ?? throw new ArgumentNullException(nameof(peerMessageListener));
            _serverContext = serverContext ?? throw new ArgumentNullException(nameof(serverContext));
            _serverProcesses = serverProcesses ?? throw new ArgumentNullException(nameof(serverProcesses));

            _peerListener.OnPeerConnected += PeerConnected;

            // TODO #364: Change after port forwarding implementation
            _peerEndPoints = config.PeerEndPoints;
        }

        #endregion

        #region IServer implementation

        /// <inheritdoc />
        public void Start()
        {
            if (_isRunning)
            {
                throw new InvalidOperationException("The server is running. To start it again please stop it before.");
            }

            _messageListenerTokenSource = new CancellationTokenSource();

            // connect to peers
            ConnectToPeers(_peerEndPoints);

            // listen for peers
            _peerListener.Start();

            _isRunning = true;

            Parallel.ForEach(_serverProcesses, sp => sp.Start());
        }

        /// <inheritdoc />
        public void Stop()
        {
            Parallel.ForEach(_serverProcesses, sp => sp.Stop());

            _peerListener.Stop();

            _messageListenerTokenSource?.Cancel();

            DisconnectPeers();

            _isRunning = false;
        }

        /// <inheritdoc />
        public void ConnectToPeers(params EndPoint[] endPoints)
        {
            var rand = new Random(Environment.TickCount);
            var connectedEndPoints = _serverContext.ConnectedPeers.Keys.ToArray();
            var preferredEndPoints = endPoints
                .Concat(_peerEndPoints)
                .Except(connectedEndPoints)
                .Distinct()
                .OrderBy(_ => rand.Next())
                .Take(Math.Max(0, _serverContext.MaxConnectedPeers - connectedEndPoints.Length))
                .ToArray();

            Parallel.ForEach(preferredEndPoints, async ep =>
            {
                try
                {
                    if (_serverContext.ConnectedPeers.ContainsKey(ep))
                    {
                        return;
                    }

                    var peer = await _peerFactory.ConnectTo(ep);
                    PeerConnected(this, peer);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Something went wrong with {ep}. Exception: {ex}");
                }
            });
        }
        #endregion

        #region IBroadcaster implementation
        /// <inheritdoc />
        public void Broadcast(Message message, IPeer source = null)
        {
            var peers = _serverContext.ConnectedPeers.Values
                .Where(p => source == null || !p.EndPoint.Equals(source.EndPoint))
                .ToArray();

            Parallel.ForEach(peers, peer => peer.Send(message));
        }
        #endregion

        #region IDisposable Implementation
        /// <inheritdoc />
        public void Dispose()
        {
            Stop();
            _peerListener.OnPeerConnected -= PeerConnected;
        }
        #endregion

        #region Private Methods 
        /// <summary>
        /// Peer connected Event
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="peer">Peer</param>
        private void PeerConnected(object sender, IPeer peer)
        {
            try
            {
                if (_acl.IsAllowed(peer.EndPoint) == false)
                {
                    throw new UnauthorizedAccessException($"The endpoint \"{peer.EndPoint}\" is prohibited by ACL.");
                }

                if (!_serverContext.ConnectedPeers.TryAdd(peer.EndPoint, peer))
                {
                    throw new InvalidOperationException($"The peer with endpoint \"{peer.EndPoint}\" is already connected.");
                }

                peer.OnDisconnect += (s, e) => _serverContext.ConnectedPeers.TryRemove(peer.EndPoint, out _);
                _peerMessageListener.StartFor(peer, _messageListenerTokenSource.Token);
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Something went wrong with {peer}. Exception: {e}");
                peer.Disconnect();
            }
        }

        /// <summary>
        /// Send disconnect to all current Peers
        /// </summary>
        private void DisconnectPeers()
        {
            Parallel.ForEach(_serverContext.ConnectedPeers.Values, peer => peer.Disconnect());
        }
        #endregion
    }
}