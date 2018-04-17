using System;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace NeoSharp.Core.Network
{
    public class TcpPeer : IPeer, IDisposable
    {
        public EndPoint EndPoint { get; }

        private readonly ILogger<TcpPeer> _logger;

        private Socket _socket;
#pragma warning disable 649
        private NetworkStream _stream;
#pragma warning restore 649
        private IPEndPoint _ipEp;
        // ReSharper disable once NotAccessedField.Local
        private uint _serverNonce;

        public TcpPeer(EndPoint endPoint, ILogger<TcpPeer> logger)
        {
            EndPoint = endPoint;
            _logger = logger;
            IPAddress.TryParse(endPoint.Host, out var ipAddr);
            _ipEp = new IPEndPoint(ipAddr, endPoint.Port);
        }

        public async Task Connect(uint serverNonce)
        {
            _serverNonce = serverNonce;
            _logger.LogInformation($"Connecting to {_ipEp}");

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            await _socket.ConnectAsync(_ipEp.Address, _ipEp.Port); // TODO: thread etc

            _logger.LogInformation($"Connected to {_ipEp}");
            //_stream = new NetworkStream(_socket);                       
        }        

        // ReSharper disable once UnusedMember.Local
        private void SendVersion()
        {
            _logger.LogInformation($"Sending version to {_ipEp}");
            _stream.WriteAsync(new byte[] { 1, 4, 5 }, 0, 3); // dummy send version
        }       

        public void Disconnect()
        {
            _logger.LogInformation("Disconnecting peer");
            _socket?.Disconnect(false);
            _socket?.Close();
        }


        public void Dispose()
        {
            // TODO: Call Disconnect()?
            _socket?.Dispose();
            _stream?.Dispose();
        }
    }
}
