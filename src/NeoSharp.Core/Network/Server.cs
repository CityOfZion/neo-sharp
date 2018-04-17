using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NeoSharp.Core.Network
{
    public class Server : IServer
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly ILogger<Server> _logger;
        private readonly NetworkConfig _cfg;
        private readonly IPeerFactory _peerFactory;

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        // ReSharper disable once CollectionNeverUpdated.Local
        private ConcurrentBag<IPeer> _connectedPeers;                // if we successfully connect with a peer it is inserted into this list
        // ReSharper disable once NotAccessedField.Local
        private IList<IPEndPoint> _failedPeers;             // if we can't connect to a peer it is inserted into this list
        private uint _nonce;                                // uniquely identifies this server so we can filter out our own messages sent back to us by other nodes

        public Server(ILogger<Server> logger, NetworkConfig networkConfig, IPeerFactory peerFactory)
        {
            _logger = logger;
            _cfg = networkConfig;
            _peerFactory = peerFactory;


            _connectedPeers = new ConcurrentBag<IPeer>();
            _failedPeers = new List<IPEndPoint>();
            var r = new Random(Environment.TickCount);
            _nonce = (uint)r.Next();
        }

        public async Task StartServer()
        {
            // connect to peers
            await ConnectToPeers();

            // receive transactions

            // listen for peers
        }

        private async Task ConnectToPeers()
        {
            var connectTasks = _cfg.PeerEndPoints
                .Select(pep => _peerFactory.Create(pep))
                .Select(p => p.Connect(_nonce).ContinueWith(t =>
                {
                    if (t.IsCompleted)
                    {
                        _connectedPeers.Add(p);
                    }
                    else
                    {
                        _logger.LogWarning($"Something goes wrong with {p.EndPoint}. Exception: {t.Exception}");
                    }
                    return t;
                }, TaskContinuationOptions.ExecuteSynchronously));

            await Task.WhenAll(connectTasks);            
        }

        public void StopServer()
        {
            // send disconnect to all current Peers
            foreach (var peer in _connectedPeers)
            {
                peer.Disconnect();
            }
        }
    }
}
