using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

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
        private uint _nonce;                                // uniquely identifies this server so we can filter out our own messages sent back to us by other nodes

        public Server(NetworkConfig config, IPeerFactory peerFactory, IPeerListener peerListener, ILogger<Server> logger)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _peerFactory = peerFactory ?? throw new ArgumentNullException(nameof(peerFactory));
            _peerListener = peerListener ?? throw new ArgumentNullException(nameof(peerListener));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _peerListener.PeerConnected += OnPeerConnected;

            _connectedPeers = new ConcurrentBag<IPeer>();
            _failedPeers = new List<IPEndPoint>();
            var r = new Random(Environment.TickCount);
            _nonce = (uint)r.Next();
        }

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
            _peerListener.PeerConnected -= OnPeerConnected;
        }

        private void OnPeerConnected(object sender, IPeer peer)
        {
            _connectedPeers.Add(peer);
            peer.Connect(_nonce);
        }

        private void ConnectToPeers()
        {
            foreach (var peerEp in _config.PeerEndPoints)
            {
                _peerFactory
                    .Create(peerEp)
                    .ContinueWith(t =>
                    {
                        if (t.IsCompletedSuccessfully)
                        {
                            OnPeerConnected(this, t.Result);
                        }
                        else
                        {
                            _logger.LogWarning($"Something goes wrong with {peerEp}. Exception: {t.Exception}");
                        }
                    }, TaskContinuationOptions.ExecuteSynchronously);
            }
        }

        private void DisconnectPeers()
        {
            foreach (var peer in _connectedPeers)
            {
                // TODO: Revise this after handshake is implemented
                peer.Disconnect();
                (peer as IDisposable)?.Dispose();
            }

            _connectedPeers.Clear();
        }
    }
}
