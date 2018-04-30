using NeoSharp.Core.Network.Tcp.Protocols;
using System.Collections.Generic;

namespace NeoSharp.Core.Network.Tcp
{
    public class TcpProtocolSelector
    {
        private readonly Dictionary<uint, TcpProtocolBase> _protocols;

        /// <summary>
        /// Constructor
        /// </summary>
        public TcpProtocolSelector()
        {
            // Set different protocols

            var v1 = new TcpProtocolV1();
            var v2 = new TcpProtocolV2();

            _protocols = new Dictionary<uint, TcpProtocolBase>
            {
                { v1.MagicHeader, v1 },
                { v2.MagicHeader, v2 }
            };

            DefaultProtocol = v2;
        }

        /// <summary>
        /// Get protocol
        /// </summary>
        /// <param name="magicNumber">Magic number</param>
        /// <returns>Return protocol or NULL</returns>
        public TcpProtocolBase GetProtocol(uint magicNumber)
        {
            return _protocols.TryGetValue(magicNumber, out var protocol) ? protocol : null;
        }

        public TcpProtocolBase DefaultProtocol { get; }
    }
}