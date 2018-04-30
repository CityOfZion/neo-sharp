using Microsoft.Extensions.Logging;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Network.Messages;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace NeoSharp.Core.Network.Tcp
{
    public class TcpPeer : IPeer, IDisposable
    {
        private readonly ILogger<TcpPeer> _logger;
        private readonly ITcpProtocol _protocol;

        private readonly Socket _socket;
#pragma warning disable 649
        private readonly NetworkStream _stream;
#pragma warning restore 649
        // private IPEndPoint _ipEp;
        // ReSharper disable once NotAccessedField.Local
        private uint _serverNonce;

        public TcpPeer(Socket socket, ILogger<TcpPeer> logger, TcpProtocolSelector protocols)
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

            _protocol = protocols.GetProtocol(header.ToUInt32(0)) ?? throw new ArgumentNullException("protocol");
        }

        public void Connect(uint serverNonce)
        {
            _serverNonce = serverNonce;

            //_stream = new NetworkStream(_socket);                       
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
                _socket.Dispose();
            }

            _stream?.Dispose();
        }

        public Task Send<TMessage>(TMessage message) where TMessage : Message, new()
        {
            using (CancellationTokenSource cancel = new CancellationTokenSource(ITcpProtocol.TimeOut))
                return _protocol.SendMessageAsync(_stream, message, cancel);
        }

        public Task<Message> Receive()
        {
            using (CancellationTokenSource cancel = new CancellationTokenSource(ITcpProtocol.TimeOut))
                return _protocol.GetMessageAsync(_stream, cancel);
        }

        public Task<TMessage> Receive<TMessage>() where TMessage : Message, new()
        {
            return new Task<TMessage>(() =>
             {
                 Task<Message> msg = Receive();
                 msg.Wait();

                 return new TMessage();
             });
        }
    }
}