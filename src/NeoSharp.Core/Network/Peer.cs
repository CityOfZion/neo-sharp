using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;

namespace NeoSharp.Core.Network
{
    public class Peer : IPeer
    {
        private readonly ILogger<Peer> _logger;

        private Socket _socket;
        private NetworkStream _stream;
        private IPEndPoint _ipEP;
        private uint _serverNonce;

        public Peer(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Peer>();
        }

        public void Connect(IPEndPoint ipEndPoint, uint serverNonce)
        {
            _ipEP = ipEndPoint;
            _serverNonce = serverNonce;

            _logger.LogInformation($"Connecting to {_ipEP.ToString()}");

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.ConnectAsync(_ipEP.Address, _ipEP.Port); // TODO thread etc

            _logger.LogInformation($"Connected to {_ipEP.ToString()}");
            //_stream = new NetworkStream(_socket);                       
        }        

        private void sendVersion()
        {
            _logger.LogInformation($"Sending version to {_ipEP.ToString()}");
            _stream.WriteAsync(new byte[] { 1, 4, 5 }, 0, 3); // dummy send version
        }       

        public void Stop()
        {
            _logger.LogInformation("Stopping peer");
            _socket?.Disconnect(false);
            _socket?.Close();
        }


    }
}
