using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;

namespace NeoSharp.Core.Network
{
    public class Server : IServer
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly ILogger<Server> _logger;
        private readonly NetworkConfig _cfg;
        private readonly Func<IPeer> _peerFactory;
        
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        // ReSharper disable once CollectionNeverUpdated.Local
        private IList<IPeer> _connectedPeers;                // if we successfully connect with a peer it is inserted into this list
        // ReSharper disable once NotAccessedField.Local
        private IList<IPEndPoint> _failedPeers;             // if we can't connect to a peer it is inserted into this list
        private uint _nonce;                                // uniquely identifies this server so we can filter out our own messages sent back to us by other nodes

        public Server(ILoggerFactory loggerFactory, NetworkConfig networkConfig, Func<IPeer> peerFactoryInit)
        {
            _logger = loggerFactory.CreateLogger<Server>();
            _cfg = networkConfig;
            _peerFactory = peerFactoryInit;

            _connectedPeers = new List<IPeer>();
            _failedPeers = new List<IPEndPoint>();
            var r = new Random();            
            _nonce = (uint)r.Next();           
        }               

        public void StartServer()
        {
            // connect to peers
            ConnectToPeers();

            // receive transactions

            // listen for peers
        }

        private void ConnectToPeers()
        {
            // private net testing setup
            var ipStr = _cfg.ServerIp;
            var port = _cfg.ServerStartPort;
            for (var i = 0; i < 4; i++)
            {
                IPAddress ipAddr;
                IPAddress.TryParse(ipStr, out ipAddr);
                var ipEp = new IPEndPoint(ipAddr, port + i);

                var newPeer = _peerFactory();
                newPeer.Connect(ipEp, _nonce);
            }
        }

        public void StopServer()
        {
            // send disconnect to all current Peers
            foreach(var peer in _connectedPeers)
            {
                peer.Stop();                
            }
        }        
    }
}
