using NeoSharp.Core.Network.Tcp.Protocols;
using System.Collections.Generic;

namespace NeoSharp.Core.Network.Tcp
{
    public class TcpProtocolSelector
    {
        /// <summary>
        /// Selector
        /// </summary>
        public readonly static TcpProtocolSelector Selector = new TcpProtocolSelector();

        Dictionary<uint, ITcpProtocol> _protocols;

        /// <summary>
        /// Constructor
        /// </summary>
        public TcpProtocolSelector()
        {
            // Set different protocols

            TcpProtocolV1 v1 = new TcpProtocolV1();
            TcpProtocolV2 v2 = new TcpProtocolV2();

            _protocols = new Dictionary<uint, ITcpProtocol>
            {
                { v1.MagicHeader, v1 },
                { v2.MagicHeader, v2 }
            };
        }

        /// <summary>
        /// Get protocol
        /// </summary>
        /// <param name="magicNumber">Magic number</param>
        /// <returns>Return protocol or NULL</returns>
        public ITcpProtocol GetProtocol(uint magicNumber)
        {
            if (_protocols.TryGetValue(magicNumber, out ITcpProtocol protocol))
                return protocol;

            return null;
        }
    }
}