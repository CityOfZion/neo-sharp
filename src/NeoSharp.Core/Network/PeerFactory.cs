using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NeoSharp.Core.DI;

namespace NeoSharp.Core.Network
{
    public class PeerFactory : IPeerFactory
    {
        private readonly IReadOnlyDictionary<Protocol, Func<EndPoint, IPeer>> _protocolSpecificPeerFactories;

        public PeerFactory(IContainer container)
        {
            _protocolSpecificPeerFactories = new Dictionary<Protocol, Func<EndPoint, IPeer>>
            {
                { Protocol.Tcp, ep => new TcpPeer(ep, container.Resolve<ILogger<TcpPeer>>()) }
            };
        }

        public IPeer Create(EndPoint endPoint) => _protocolSpecificPeerFactories[endPoint.Protocol](endPoint);
    }
}