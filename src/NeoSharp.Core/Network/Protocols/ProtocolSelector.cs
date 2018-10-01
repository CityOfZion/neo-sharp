using System;
using NeoSharp.Core.Messaging.Messages;

namespace NeoSharp.Core.Network.Protocols
{
    public class ProtocolSelector
    {
        /// <summary>
        /// Contains the default protocol
        /// </summary>
        public readonly ProtocolBase DefaultProtocol;
        /// <summary>
        /// Contains the list of the protocols
        /// </summary>
        private readonly Func<VersionPayload, ProtocolBase>[] _protocols;

        /// <summary>
        /// Constructor
        /// </summary>
        public ProtocolSelector(NetworkConfig config)
        {
            // Set different protocols

            var v1 = new ProtocolV1(config);
            var v2 = new ProtocolV2(config);

            _protocols = new Func<VersionPayload, ProtocolBase>[]
            {
                new Func<VersionPayload,ProtocolBase>
                (
                    // TODO #368: I don't know if we will use Version or Services
                    (v) => v.Version == 2 ? v2 : null
                )
            };

            // Default protocol, oficial protocol

            DefaultProtocol = v1;
        }

        /// <summary>
        /// Get protocol
        /// </summary>
        /// <param name="version">Version</param>
        /// <returns>Return protocol or NULL</returns>
        public ProtocolBase GetProtocol(VersionPayload version)
        {
            // Search for protocol or return default

            foreach (var query in _protocols)
            {
                var proto = query(version);
                if (proto != null) return proto;
            }

            return DefaultProtocol;
        }
    }
}