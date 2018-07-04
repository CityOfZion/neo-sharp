using System;
using System.Linq;
using System.Threading.Tasks;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class AddrMessageMessageHandler : IMessageHandler<AddrMessage>
    {
        #region Variables

        private readonly IServer _server;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public AddrMessageMessageHandler(IServer server)
        {
            _server = server ?? throw new ArgumentNullException(nameof(server));
        }

        public Task Handle(AddrMessage message, IPeer sender)
        {
            var connectedEndPoints = _server.ConnectedPeers
                .Select(p => p.EndPoint)
                .ToArray();

            var endPointsToConnect = message.Payload.Address
                .Select(nat => new EndPoint(Protocol.Tcp, nat.EndPoint))
                .Where(ep => !connectedEndPoints.Contains(ep))
                .ToArray();

            _server.ConnectToPeers(endPointsToConnect);

            return Task.CompletedTask;
        }
    }
}