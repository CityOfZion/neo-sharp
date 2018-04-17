using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;

namespace NeoSharp.Core.Network
{
    public class NetworkConfig 
    {
        private static readonly Regex _peerEndPointPattern = new Regex(@"^(?<proto>\w+)://(?<host>[^/:]+):?(?<port>\d+)?/?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public EndPoint[] PeerEndPoints { get; private set; }

        public NetworkConfig(IConfiguration configuration = null)
        {
            var peerEndPoints = configuration?.GetSection(nameof(NetworkConfig))?.GetSection("PeerEndPoints").Get<string[]>();
            if (peerEndPoints == null) return;

            string MatchGroupValue(Group group) => group.Success ? group.Value : null;

            PeerEndPoints = peerEndPoints
                .Select(pep => _peerEndPointPattern.Match(pep))
                .Where(m => m.Success)
                .Select(m => ParseEndPoint(MatchGroupValue(m.Groups["proto"]), MatchGroupValue(m.Groups["host"]), MatchGroupValue(m.Groups["port"])))
                .ToArray();
        }

        private EndPoint ParseEndPoint(string protocol, string host, string port)
        {
            // TODO: Is it?
            if (protocol == null) protocol = "tcp";

            if (host == null) return null;

            // TODO: More safe parsing
            return new EndPoint
            {
                Protocol = Enum.Parse<Protocol>(protocol, true),
                Host = host,
                Port = int.Parse(port)
            };
        }
    }
}
