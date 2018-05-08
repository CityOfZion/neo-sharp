using Microsoft.Extensions.Configuration;

namespace NeoSharp.Core.Network
{
    public class NetworkConfig
    {
        public uint Magic { get; set; }
        public ushort Port { get; internal set; }
        public bool ForceIPv6 { get; internal set; }
        public EndPoint[] PeerEndPoints { get; internal set; }

        public NetworkConfig(IConfiguration configuration = null)
        {
            PeerEndPoints = new EndPoint[0];
            configuration?.GetSection("network")?.Bind(this);
        }
    }
}
