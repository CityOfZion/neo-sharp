using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;

namespace NeoSharp.Core.Network
{
    public class Peer : IPeer
    {
        private readonly ILogger<Peer> _logger;

        private Socket _socket;
#pragma warning disable 649
        private NetworkStream _stream;
#pragma warning restore 649
        private IPEndPoint _ipEp;
        // ReSharper disable once NotAccessedField.Local
        private uint _serverNonce;

        public Peer(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Peer>();
        }

        public void Connect(IPEndPoint ipEndPoint, uint serverNonce)
        {
            _ipEp = ipEndPoint;
            _serverNonce = serverNonce;

            _logger.LogInformation($"Connecting to {_ipEp}");

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.ConnectAsync(_ipEp.Address, _ipEp.Port); // TODO: thread etc

            _logger.LogInformation($"Connected to {_ipEp}");
            //_stream = new NetworkStream(_socket);                       
        }        

        // ReSharper disable once UnusedMember.Local
        private void SendVersion()
        {
            _logger.LogInformation($"Sending version to {_ipEp}");
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
