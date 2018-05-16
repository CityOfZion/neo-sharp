using System;
using System.Linq;
using System.Net;
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
            networkConfig.Magic = ParseUInt32(config, "magic", DefaultMagic);
            networkConfig.Port = ParseUInt16(config, "port");
            networkConfig.ForceIPv6 = ParseBool(config, "forceIPv6");
            networkConfig.PeerEndPoints = ParsePeerEndPoints(config);
        }

        public static void Bind(this IConfiguration config, RpcConfig rpcConfig)
        {
            rpcConfig.ListenEndPoint = ParseIpEndPoint(config, "listenEndPoint");

            rpcConfig.SSL = new RpcConfig.SSLCert();

            var v = config?.GetSection("SSL");
            rpcConfig.SSL.Path = ParseString(v, "path");
            rpcConfig.SSL.Password = ParseString(v, "password");
        }

        private static uint ParseUInt32(IConfiguration config, string section, uint def = 0)
        {
            var magic = config.GetSection(section)?.Get<uint>();
            return magic ?? def;
        }

        private static ushort ParseUInt16(IConfiguration config, string section, ushort def = 0)
        {
            var port = config.GetSection(section)?.Get<ushort>();
            return port ?? def;
        }

        private static bool ParseBool(IConfiguration config, string section, bool def = true)
        {
            var forceIPv6 = config.GetSection(section)?.Get<bool>();
            return forceIPv6 ?? def;
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

        private static string ParseString(IConfiguration config, string section)
        {
            return config.GetSection(section)?.Get<string>();
        }

        private static IPEndPoint ParseIpEndPoint(IConfiguration config, string section)
        {
            string host = ParseString(config, section);
            if (host == null) return null;

            string[] split = host.Split(',', StringSplitOptions.RemoveEmptyEntries);

            // TODO: More safe parsing
            return new IPEndPoint(IPAddress.Parse(split[0]), Convert.ToInt32(split[1]));
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