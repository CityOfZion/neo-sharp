using System;
using System.ComponentModel;
using System.Net;
using System.Text.RegularExpressions;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Converters;
using NeoSharp.Core.Exceptions;

namespace NeoSharp.Core.Network
{
    [TypeConverter(typeof(EndPointConverter))]
    [BinaryTypeSerializer(typeof(EndPointConverter))]
    public class EndPoint : IEquatable<EndPoint>
    {
        private static readonly Regex EndPointPattern =
            new Regex(@"^(?<proto>\w+)://(?<address>[^/]+)/?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Protocol
        /// </summary>
        public Protocol Protocol { get; set; }

        /// <summary>
        /// Host
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Port
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public EndPoint() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="protocol">Protocol</param>
        /// <param name="ipEndPoint">IP Endpoint</param>
        public EndPoint(Protocol protocol, IPEndPoint ipEndPoint)
        {
            Protocol = protocol;
            Host = ipEndPoint.Address.ToString();
            Port = ipEndPoint.Port;
        }

        /// <summary>
        /// Get IpEndPoint
        /// </summary>
        public IPEndPoint ToIpEndPoint()
        {
            var uriType = Uri.CheckHostName(Host);
            if (uriType == UriHostNameType.Dns)
            {
                //check dns
                var hostEntry = Dns.GetHostEntry(Host);
                if (hostEntry.AddressList.Length == 0) return null;
            }
            IPAddress ipAddress;
            IPEndPoint ipEndPoint = null;
            if (IPAddress.TryParse(Host, out ipAddress))
            {
                ipEndPoint = new IPEndPoint(ipAddress, Port);
            }
            return ipEndPoint;
        }

        public static EndPoint Parse(string value)
        {
            var match = EndPointPattern.Match(value);
            if (!match.Success)
            {
                throw new InvalidEndpointException("The endpoint has an invalid format.");
            }

            return Parse(
                MatchGroupValue(match.Groups["proto"]),
                MatchGroupValue(match.Groups["address"]));
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Protocol}://{Host}:{Port}".ToLowerInvariant();
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /// <inheritdoc />
        public bool Equals(EndPoint other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return string.Equals(ToString(), other.ToString());
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;

            return Equals((EndPoint)obj);
        }

        private static string MatchGroupValue(Group group) => group.Success ? group.Value : null;

        private static EndPoint Parse(string protocol, string address)
        {
            var host = address;
            var port = 0;

            var portIndex = address.LastIndexOf(':');
            if (portIndex > 0 &&
                portIndex < address.Length - 1 &&
                address[portIndex - 1] != ':')
            {
                host = address.Substring(0, portIndex);
                port = int.Parse(address.Substring(portIndex + 1));
            }

            if (!Enum.TryParse(typeof(Protocol), protocol, true, out var protocolEnum))
            {
                protocolEnum = Protocol.Unknown;
            }

            return new EndPoint
            {
                Protocol = (Protocol)protocolEnum,
                Host = host,
                Port = port
            };
        }
    }

    public enum Protocol
    {
        Unknown = 0,
        Tcp = 1,
        // Tls = 2,
        // Ws = 3,
        // Wss = 4
    }
}