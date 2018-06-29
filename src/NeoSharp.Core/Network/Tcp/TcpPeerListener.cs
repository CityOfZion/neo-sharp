using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using NeoSharp.Core.Logging;

namespace NeoSharp.Core.Network.Tcp
{
    public class TcpPeerListener : IPeerListener, IDisposable
    {
        private readonly ITcpPeerFactory _peerFactory;
        private readonly ILogger<TcpPeerListener> _logger;
        private readonly TcpListener _listener;

        public event EventHandler<IPeer> OnPeerConnected;

        public TcpPeerListener(
            NetworkConfig config,
            ITcpPeerFactory peerFactory,
            ILogger<TcpPeerListener> logger)
        {
            _peerFactory = peerFactory;
            _logger = logger;
            _listener = new TcpListener(IPAddress.Any, config.Port);
            _listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
        }

        public void Start()
        {
            Stop();

            _listener.Start();

            Task.Factory.StartNew(AcceptPeers, TaskCreationOptions.LongRunning);
        }

        public void Stop()
        {
            _listener.Stop();
        }

        public void Dispose()
        {
            Stop();
        }

        private async Task AcceptPeers()
        {
            while (true)
            {
                Socket socket;

                try
                {
                    socket = await _listener.AcceptSocketAsync();

                    var ipEndPoint = (IPEndPoint)socket.RemoteEndPoint;

                    _logger.LogInformation($"\"{ipEndPoint.Address}\" is connected");
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch (SocketException)
                {
                    continue;
                }

                var peer = _peerFactory.CreateFrom(socket);

                OnPeerConnected?.Invoke(this, peer);
            }
        }
    }
}