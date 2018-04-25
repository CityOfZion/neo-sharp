using System;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;

namespace NeoSharp.Core.Network.Tcp
{
    public class TcpPeer : IPeer, IDisposable
    {
        private readonly ILogger<TcpPeer> _logger;

        private Socket _socket;
#pragma warning disable 649
        private NetworkStream _stream;
#pragma warning restore 649
        // private IPEndPoint _ipEp;
        // ReSharper disable once NotAccessedField.Local
        private uint _serverNonce;

        public TcpPeer(Socket socket, ILogger<TcpPeer> logger)
        {
            _socket = socket ?? throw new ArgumentNullException(nameof(socket));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Connect(uint serverNonce)
        {
            _serverNonce = serverNonce;

            //_stream = new NetworkStream(_socket);                       
        }        

        // ReSharper disable once UnusedMember.Local
        private void SendVersion()
        {
            // _logger.LogInformation($"Sending version to {_ipEp}");
            _stream.WriteAsync(new byte[] { 1, 4, 5 }, 0, 3); // dummy send version
        }       

        public void Disconnect()
        {
            _logger.LogInformation("Disconnecting peer");
        }

        public void Dispose()
        {
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Dispose();
            _stream?.Dispose();
        }
    }
}
