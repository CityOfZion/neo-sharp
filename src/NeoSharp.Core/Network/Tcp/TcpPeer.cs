using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NeoSharp.Core.Extensions;

namespace NeoSharp.Core.Network.Tcp
{
    public class TcpPeer : IPeer, IDisposable
    {
        private readonly ILogger<TcpPeer> _logger;
        private readonly ITcpProtocol _protocol;

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

            _stream = new NetworkStream(socket, true);

            // Select protocol by the first 4 bytes

            byte[] header = new byte[4];
            using (CancellationTokenSource cancel = new CancellationTokenSource(ITcpProtocol.TimeOut))
            {
                Task ret = _stream.ReadAsync(header, 0, 4, cancel.Token);
                ret.Wait();
            }

            _protocol = TcpProtocolSelector.Selector.
                GetProtocol(header.ToUInt32(0)) ?? throw new ArgumentNullException("protocol");
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
            if (_socket != null)
            {
                _socket.Shutdown(SocketShutdown.Both);
                _socket?.Dispose();
            }

            _stream?.Dispose();
        }
    }
}
