using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Network.Messages;

namespace NeoSharp.Core.Network
{
    public class Server : IServer, IDisposable
    {
        private readonly NetworkConfig _config;
        private readonly IPeerFactory _peerFactory;
        private readonly IPeerListener _peerListener;
        private readonly ILogger<Server> _logger;

        private readonly ConcurrentBag<IPeer> _connectedPeers;                // if we successfully connect with a peer it is inserted into this list
        // ReSharper disable once NotAccessedField.Local
        private IList<IPEndPoint> _failedPeers;             // if we can't connect to a peer it is inserted into this list
        private readonly ushort _port;
        private readonly uint _nonce;                                // uniquely identifies this server so we can filter out our own messages sent back to us by other nodes
        private readonly string _userAgent;       
        private readonly EventHandler<IPeer> _peerConnected = async (s, e) => await ((Server)s).PeerConnected(e);

        public Server(NetworkConfig config, IPeerFactory peerFactory, IPeerListener peerListener, ILogger<Server> logger)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _peerFactory = peerFactory ?? throw new ArgumentNullException(nameof(peerFactory));
            _peerListener = peerListener ?? throw new ArgumentNullException(nameof(peerListener));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _peerListener.OnPeerConnected += _peerConnected;

            _connectedPeers = new ConcurrentBag<IPeer>();
            _failedPeers = new List<IPEndPoint>();

            // TODO: Change after port forwarding implementation
            _port = _config.Port;

            var r = new Random(Environment.TickCount);
            _nonce = (uint)r.Next();

            _userAgent = $"/NEO:{Assembly.GetExecutingAssembly().GetName().Version.ToString(3)}/";
        }

        public IReadOnlyCollection<IPeer> ConnectedPeers => _connectedPeers;

        public void Start()
        {
            Stop();

            // connect to peers
            ConnectToPeers();

            // receive transactions

            // listen for peers
            _peerListener.Start();
        }

        public void Stop()
        {
            _peerListener.Stop();

            // send disconnect to all current Peers
            DisconnectPeers();
        }

        public void Dispose()
        {
            Stop();
            _peerListener.OnPeerConnected -= _peerConnected;
        }

        private async Task PeerConnected(IPeer peer)
        {
            try
            {
                await Handshake(peer);
                _connectedPeers.Add(peer);
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
                    .ContinueWith(async t =>
                    {
                        if (t.IsCompletedSuccessfully)
                        {
                            await PeerConnected(t.Result);
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

        private async Task Handshake(IPeer peer)
        {
            var version = GetVersionMessage();

            await peer.Send(version);

            var peerVersion = await peer.Receive<VersionMessage>();
            if (version.Payload.Nonce != peerVersion.Payload.Nonce)
            {
                throw new InvalidOperationException("The handshake failed.");
            }

            await peer.Send<VersionAcknowledgmentMessage>();
            await peer.Receive<VersionAcknowledgmentMessage>();
        }

        private VersionMessage GetVersionMessage()
        {
            var version = new VersionMessage();
            version.Payload.Version = 0;
            // TODO: What's it?
            // version.Payload.Services = NetworkAddressWithTime.NODE_NETWORK;
            version.Payload.Timestamp = DateTime.Now.ToTimestamp();
            version.Payload.Port = _port;
            version.Payload.Nonce = _nonce;
            version.Payload.UserAgent = _userAgent;
            // TODO: Inject blockchain and get height
            // version.Payload.StartHeight = Blockchain.Default?.Height ?? 0;
            version.Payload.Relay = true;

            return version;
        }
    }
}
