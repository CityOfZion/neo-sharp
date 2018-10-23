using System;
using System.Linq;
using System.Net;
using Microsoft.Extensions.Configuration;
using NeoSharp.Core.Network.Rpc;
using NeoSharp.Core.Network.Security;

namespace NeoSharp.Core.Network
{
    public static class ConfigurationExtensions
    {
        private const int DefaultMagic = 7630401;

        public static void Bind(this IConfiguration config, NetworkConfig networkConfig)
        {
            networkConfig.Magic = ParseUInt32(config, "magic", DefaultMagic);
            networkConfig.Port = ParseUInt16(config, "port");
            networkConfig.ForceIPv6 = ParseBool(config, "forceIPv6");
            networkConfig.MaxConnectedPeers = ParseUInt16(config, "maxConnectedPeers");
            networkConfig.AclConfig = ParseAcl(config, "acl");
            networkConfig.PeerEndPoints = ParsePeerEndPoints(config);
            networkConfig.StandByValidator = ParseStandByValidators(config);
        }

        public static void Bind(this IConfiguration config, RpcConfig rpcConfig)
        {
            rpcConfig.ListenEndPoint = ParseIpEndPoint(config, "listenEndPoint");

            rpcConfig.Ssl = new RpcConfig.SslCert();

            var sslSection = config?.GetSection("ssl");
            rpcConfig.Ssl.Path = ParseString(sslSection, "path");
            rpcConfig.Ssl.Password = ParseString(sslSection, "password");
            rpcConfig.AclConfig = ParseAcl(config, "acl");
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
            var peerEndPoints = config.GetSection("peerEndPoints")?.Get<string[]>().Distinct();
            if (peerEndPoints == null) return new EndPoint[0];

            return peerEndPoints
                .Select(EndPoint.Parse)
                .ToArray();
        }

        private static string[] ParseStandByValidators(this IConfiguration config)
        {
            var standByValidators = config.GetSection("standByValidators")?.Get<string[]>().Distinct();
            if (standByValidators == null) return new string[0];

            return standByValidators.ToArray();
        }

        private static IPEndPoint ParseIpEndPoint(IConfiguration config, string section)
        {
            var host = ParseString(config, section);
            if (host == null) return null;

            var split = host.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (split.Length != 2) return null;

            if (IPAddress.TryParse(split[0], out var ipAddress) && int.TryParse(split[1], out var port))
            {
                return new IPEndPoint(ipAddress, port);
            }

            return null;
        }

        private static NetworkAclConfig ParseAcl(IConfiguration config, string section)
        {
            var acl = new NetworkAclConfig();

            var aclSection = config?.GetSection(section);

            acl.Path = ParseString(aclSection, "path");
            acl.Type = ParseEnum(aclSection, "type", NetworkAclType.None);

            return acl;
        }
    }
}