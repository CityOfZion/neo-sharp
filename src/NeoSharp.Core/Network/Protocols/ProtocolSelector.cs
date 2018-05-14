using NeoSharp.BinarySerialization;
using System.Collections.Generic;

namespace NeoSharp.Core.Network.Protocols
{
    public class ProtocolSelector
    {
        private readonly IReadOnlyDictionary<uint, ProtocolBase> _protocols;

        /// <summary>
        /// Constructor
        /// </summary>
        public ProtocolSelector(NetworkConfig config, IBinarySerializer serializer, IBinaryDeserializer deserializer)
        {
            // Set different protocols

            var v1 = new ProtocolV1(config, serializer, deserializer);
            var v2 = new ProtocolV2(config, serializer, deserializer);

            _protocols = new Dictionary<uint, ProtocolBase>
            {
                { v1.Version, v1 },
                { v2.Version, v2 }
            };

            DefaultProtocol = v2;
        }

        /// <summary>
        /// Get protocol
        /// </summary>
        /// <param name="version">Version</param>
        /// <returns>Return protocol or NULL</returns>
        public ProtocolBase GetProtocol(uint version)
        {
            return _protocols.TryGetValue(version, out var protocol) ? protocol : null;
        }

        public ProtocolBase DefaultProtocol { get; }
    }
}