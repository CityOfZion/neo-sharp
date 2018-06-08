using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NeoSharp.Core.Network.Tcp;

namespace NeoSharp.Core.Network
{
    public class PeerFactory : IPeerFactory
    {
        /// <summary>
        /// Cache specific peer factories
        /// </summary>
        private readonly IReadOnlyDictionary<Protocol, Func<EndPoint, Task<IPeer>>> _protocolSpecificPeerFactories;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tcpPeerFactory">Tcp factory</param>
        public PeerFactory(ITcpPeerFactory tcpPeerFactory)
        {
            _protocolSpecificPeerFactories = new Dictionary<Protocol, Func<EndPoint, Task<IPeer>>>
            {
                { Protocol.Tcp, tcpPeerFactory.ConnectTo }
            };
        }

        /// <summary>
        /// Connect to
        /// </summary>
        /// <param name="endPoint">Endpoint</param>
        /// <returns>Peer</returns>
        public Task<IPeer> ConnectTo(EndPoint endPoint)
        {
            if (endPoint == null)
            {
                throw new ArgumentNullException(nameof(endPoint));
            }

            if (!_protocolSpecificPeerFactories.TryGetValue(endPoint.Protocol, out Func<EndPoint, Task<IPeer>> value))
            {
                throw new NotSupportedException($"The protocol \"{endPoint.Protocol}\" is not supported.");
            }

            return value(endPoint);
        }
    }
}