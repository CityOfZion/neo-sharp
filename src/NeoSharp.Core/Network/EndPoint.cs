using System;
using System.Net;
using System.Text.RegularExpressions;

namespace NeoSharp.Core.Network
{
    public class EndPoint : IEquatable<EndPoint>
    {
        private static readonly Regex EndPointPattern =
            new Regex(@"^(?<proto>\w+)://(?<address>[^/]+)/?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly string DefaultProtocol = Protocol.Tcp.ToString();

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
        /// <param name="endPoint">Endpoint</param>
        public EndPoint(Protocol protocol, IPEndPoint endPoint)
        {
            Protocol = protocol;
            Host = endPoint.Address.ToString();
            Port = endPoint.Port;
        }

        /// <summary>
        /// Get IpEndPoint
        /// </summary>
        public IPEndPoint ToEndPoint()
        {
            // TODO: check dns

            return new IPEndPoint(IPAddress.Parse(Host), Port);
        }

        public static EndPoint Parse(string value)
        {
            var match = EndPointPattern.Match(value);
            if (!match.Success)
            {
                throw new FormatException("The enpoint has an invalid format.");
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
            if (protocol == null) protocol = DefaultProtocol;

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

            // TODO: More safe parsing
            return new EndPoint
            {
                Protocol = Enum.Parse<Protocol>(protocol, true),
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