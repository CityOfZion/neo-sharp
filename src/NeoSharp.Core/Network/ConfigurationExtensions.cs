using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;

namespace NeoSharp.Core.Network
{
    public static class ConfigurationExtensions
    {
        private const int DefaultMagic = 7630401;
        private const string DefaultProtocol = "tcp";

        private static readonly Regex _peerEndPointPattern = new Regex(@"^(?<proto>\w+)://(?<host>[^/:]+):?(?<port>\d+)?/?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static void Bind(this IConfiguration config, NetworkConfig networkConfig)
        {
            networkConfig.Magic = ParseMagic(config);
            networkConfig.Port = ParsePort(config);
            networkConfig.ForceIPv6 = ParseForceIPv6(config);
            networkConfig.PeerEndPoints = ParsePeerEndPoints(config);
        }

        private static uint ParseMagic(IConfiguration config)
        {
            var magic = config.GetSection("magic")?.Get<uint>();
            return magic ?? DefaultMagic;
        }

        private static ushort ParsePort(IConfiguration config)
        {
            var port = config.GetSection("port")?.Get<ushort>();
            return port ?? 0;
        }

        private static bool ParseForceIPv6(IConfiguration config)
        {
            var forceIPv6 = config.GetSection("forceIPv6")?.Get<bool>();
            return forceIPv6 ?? true;
        }

        private static EndPoint[] ParsePeerEndPoints(IConfiguration config)
        {
            var peerEndPoints = config.GetSection("peerEndPoints")?.Get<string[]>();
            if (peerEndPoints == null) return new EndPoint[0];

            string MatchGroupValue(Group group) => group.Success ? group.Value : null;

            return peerEndPoints
                .Select(pep => _peerEndPointPattern.Match(pep))
                .Where(m => m.Success)
                .Select(m => ParseEndPoint(
                    MatchGroupValue(m.Groups["proto"]),
                    MatchGroupValue(m.Groups["host"]),
                    MatchGroupValue(m.Groups["port"])))
                .ToArray();
        }

        private static EndPoint ParseEndPoint(string protocol, string host, string port)
        {
            if (protocol == null) protocol = DefaultProtocol;
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