using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NeoSharp.Core.Network.Tcp;

namespace NeoSharp.Core.Network
{
    public class PeerFactory : IPeerFactory
    {
        private readonly IReadOnlyDictionary<Protocol, Func<EndPoint, Task<IPeer>>> _protocolSpecificPeerFactories;

        public PeerFactory(ITcpPeerFactory tcpPeerFactory)
        {
            _protocolSpecificPeerFactories = new Dictionary<Protocol, Func<EndPoint, Task<IPeer>>>
            {
                { Protocol.Tcp, tcpPeerFactory.ConnectTo }
            };
        }

        public Task<IPeer> ConnectTo(EndPoint endPoint)
        {
            if (endPoint == null)
            {
                throw new ArgumentNullException(nameof(endPoint));
            }

            if (_protocolSpecificPeerFactories.ContainsKey(endPoint.Protocol) == false)
            {
                throw new NotSupportedException($"The protocol \"{endPoint.Protocol}\" is not supported.");
            }

            return _protocolSpecificPeerFactories[endPoint.Protocol](endPoint);
        }
    }
}