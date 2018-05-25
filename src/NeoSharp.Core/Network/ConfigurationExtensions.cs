using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using NeoSharp.Core.Network.Rpc;
using NeoSharp.Core.Network.Security;

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
            networkConfig.Acl = ParseAcl(config, "acl");
        }

        public static void Bind(this IConfiguration config, RpcConfig rpcConfig)
        {
            rpcConfig.ListenEndPoint = ParseIpEndPoint(config, "listenEndPoint");

            rpcConfig.Ssl = new RpcConfig.SslCert();

            var sslSection = config?.GetSection("ssl");
            rpcConfig.Ssl.Path = ParseString(sslSection, "path");
            rpcConfig.Ssl.Password = ParseString(sslSection, "password");
            rpcConfig.Acl = ParseAcl(config, "acl");
        }

        private static uint ParseUInt32(IConfiguration config, string section, uint defaultValue = 0)
        {
            var value = config.GetSection(section)?.Get<uint>();
            return value ?? defaultValue;
        }

        private static ushort ParseUInt16(IConfiguration config, string section, ushort defaultValue = 0)
        {
            var value = config.GetSection(section)?.Get<ushort>();
            return value ?? defaultValue;
        }

        private static bool ParseBool(IConfiguration config, string section, bool defaultValue = true)
        {
            var value = config.GetSection(section)?.Get<bool>();
            return value ?? defaultValue;
        }

        private static string ParseString(IConfiguration config, string section)
        {
            return config.GetSection(section)?.Get<string>();
        }

        private static TEnum ParseEnum<TEnum>(IConfiguration config, string section, TEnum defaultValue = default(TEnum)) where TEnum : struct
        {
            var str = ParseString(config, section);

            if (string.IsNullOrEmpty(str))
                return defaultValue;

            if (Enum.TryParse(str, out TEnum res))
                return res;

            return defaultValue;
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

        private static IPEndPoint ParseIpEndPoint(IConfiguration config, string section)
        {
            var host = ParseString(config, section);
            if (host == null) return null;

            var split = host.Split(',', StringSplitOptions.RemoveEmptyEntries);

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

        private static NetworkAclConfig ParseAcl(IConfiguration config, string section)
        {
            var acl = new NetworkAclConfig();

            var aclSection = config?.GetSection(section);

            acl.Path = ParseString(aclSection, "path");
            acl.Type = ParseEnum(aclSection, "type", NetworkAclConfig.AclType.None);

            return acl;
        }
    }
}