using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using NeoSharp.Core.Network.Security;

namespace NeoSharp.Core.Network
{
    public class NetworkConfig
    {
        /// <summary>
        /// Magic number
        /// </summary>
        public uint Magic { get; internal set; }
        /// <summary>
        /// Portt
        /// </summary>
        public ushort Port { get; internal set; }
        /// <summary>
        /// Force Ipv6
        /// </summary>
        public bool ForceIPv6 { get; internal set; }
        /// <summary>
        /// Peers
        /// </summary>
        public EndPoint[] PeerEndPoints { get; internal set; }
        /// <summary>
        /// Acl Config
        /// </summary>
        public NetworkAclConfig AclConfig { get; internal set; }
        /// <summary>
        /// StandByValidator config
        /// </summary>
        public string[] StandByValidator { get; internal set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">Configuration</param>
        public NetworkConfig(IConfiguration configuration)
        {
            PeerEndPoints = new EndPoint[0];
            configuration?.GetSection("network")?.Bind(this);
        }
    }
}