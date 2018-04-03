using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;

namespace NeoSharp.Core.Network
{
    public class Server : IServer
    {
        private readonly ILogger<Server> _logger;
        private readonly NetworkConfig _cfg;
        private readonly Func<IPeer> _peerFactory;
        
        private IList<IPeer> _connectedPeers;                // if we successfully connect with a peer it is inserted into this list
        private IList<IPEndPoint> _failedPeers;             // if we can't connect to a peer it is inserted into this list
        private uint _nonce;                                // uniquely identifies this server so we can filter out our own messages sent back to us by other nodes

        public Server(ILoggerFactory loggerFactory, NetworkConfig networkConfig, Func<IPeer> peerFactoryInit)
        {
            _logger = loggerFactory.CreateLogger<Server>();
            _cfg = networkConfig;
            _peerFactory = peerFactoryInit;

            _connectedPeers = new List<IPeer>();
            _failedPeers = new List<IPEndPoint>();
            Random r = new Random();            
            _nonce = (uint)r.Next();           
        }               

        public void StartServer()
        {
            // connect to peers
            connectToPeers();

            // receive transactions

            // listen for peers
        }

        private void connectToPeers()
        {
            // private net testing setup
            String ipStr = _cfg.ServerIp;
            int port = _cfg.ServerStartPort;
            for (int i = 0; i < 4; i++)
            {
                IPAddress ipAddr;
                IPAddress.TryParse(ipStr, out ipAddr);
                IPEndPoint ipEP = new IPEndPoint(ipAddr, port + i);

                IPeer newPeer = _peerFactory();
                newPeer.Connect(ipEP, _nonce);
            }
        }

        public void StopServer()
        {
            // send disconnect to all current Peers
            foreach(IPeer peer in _connectedPeers)
            {
                peer.Stop();                
            }
        }        
    }
}
