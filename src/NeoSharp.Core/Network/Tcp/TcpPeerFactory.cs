using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Network.Protocols;

namespace NeoSharp.Core.Network.Tcp
{
    public class TcpPeerFactory : ITcpPeerFactory
    {
        #region Variables

        private readonly bool _forceIPv6;
        private readonly ILogger<TcpPeerFactory> _logger;
        private readonly ILogger<TcpPeer> _peerLogger;
        private readonly ProtocolSelector _protocolSelector;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config">Configuration</param>
        /// <param name="protocolSelector">Protocol selector</param>
        /// <param name="logger">TcpPeerFactory Logger</param>
        /// <param name="peerLogger">TcpPeer logger</param>
        public TcpPeerFactory(
            NetworkConfig config,
            ProtocolSelector protocolSelector,
            ILogger<TcpPeerFactory> logger,
            ILogger<TcpPeer> peerLogger)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            _forceIPv6 = config.ForceIPv6;
            _protocolSelector = protocolSelector ?? throw new ArgumentNullException(nameof(protocolSelector));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _peerLogger = peerLogger ?? throw new ArgumentNullException(nameof(peerLogger));
        }

        /// <summary>
        /// Connect to
        /// </summary>
        /// <param name="endPoint">Endpoint</param>
        /// <returns>IPeer</returns>
        public async Task<IPeer> ConnectTo(EndPoint endPoint)
        {
            var ipAddress = await GetIpAddress(endPoint.Host);
            if (ipAddress == null)
            {
                throw new InvalidOperationException($"\"{endPoint.Host}\" cannot be resolved to an ip address.");
            }

            if (_forceIPv6)
            {
                ipAddress = ipAddress.MapToIPv6();
            }
            else if (ipAddress.IsIPv4MappedToIPv6)
            {
                ipAddress = ipAddress.MapToIPv4();
            }

            var ipEp = new IPEndPoint(ipAddress, endPoint.Port);

            _logger.LogInformation($"Connecting to {ipEp}...");

            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            await socket.ConnectAsync(ipEp.Address, ipEp.Port);

            _logger.LogInformation($"Connected to {ipEp}");

            return CreateFrom(socket);
        }

        /// <summary>
        /// Get Ip from hostname or address
        /// </summary>
        /// <param name="hostNameOrAddress">Host or Address</param>
        /// <returns>IpAdress</returns>
        private static async Task<IPAddress> GetIpAddress(string hostNameOrAddress)
        {
            if (IPAddress.TryParse(hostNameOrAddress, out var ipAddress))
            {
                return ipAddress;
            }

            try
            {
                var ipHostEntry = await Dns.GetHostEntryAsync(hostNameOrAddress);

                return ipHostEntry.AddressList
                    .FirstOrDefault(p => p.AddressFamily == AddressFamily.InterNetwork || p.IsIPv6Teredo);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Create Peer from socket
        /// </summary>
        /// <param name="socket">Socket</param>
        /// <returns>TcpPeer</returns>
        public TcpPeer CreateFrom(Socket socket)
        {
            // TODO: thread etc
            return new TcpPeer(socket, _protocolSelector, _peerLogger);
        }
    }
}