using System;
using System.Linq;
using System.Threading.Tasks;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class GetAddrMessageMessageHandler : MessageHandler<GetAddrMessage>
    {
        #region Private fields
        private const int MaxCountToSend = 200;

        private readonly IServer _server;
        #endregion

        #region Constructor 
        public GetAddrMessageMessageHandler(IServer server)
        {
            // TODO #433: Replace IServer with IServerContext
            _server = server ?? throw new ArgumentNullException(nameof(server));
        }
        #endregion

        #region MessageHandler override methods
        /// <inheritdoc />
        public override async Task Handle(GetAddrMessage message, IPeer sender)
        {
            var peers = _server.ConnectedPeers
                .OrderBy(p => BitConverter.ToUInt32(Crypto.Default.GenerateRandomBytes(4), 0))
                .Take(MaxCountToSend)
                .ToArray();

            var networkAddressWithTimes = peers
                .Select(p => new NetworkAddressWithTime
                {
                    EndPoint = p.EndPoint.ToIpEndPoint(),
                    Services = p.Version.Services,
                    Timestamp = p.Version.Timestamp
                }
                )
                .ToArray();

            await sender.Send
            (
                new AddrMessage(networkAddressWithTimes)
            );
        }

        /// <inheritdoc />
        public override bool CanHandle(Message message)
        {
            return message is GetAddrMessage;
        }
        #endregion
    }
}