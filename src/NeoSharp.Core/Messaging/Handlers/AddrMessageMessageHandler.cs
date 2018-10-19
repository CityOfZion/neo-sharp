using System;
using System.Linq;
using System.Threading.Tasks;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class AddrMessageMessageHandler : MessageHandler<AddrMessage>
    {
        #region Private Fields 
        private readonly IServer _server;
        private readonly IServerContext _serverContext;

        #endregion

        #region Constructor
        public AddrMessageMessageHandler(IServer server, IServerContext serverContext)
        {
            _server = server ?? throw new ArgumentNullException(nameof(server));
            _serverContext = serverContext ?? throw new ArgumentNullException(nameof(serverContext));
        }
        #endregion

        #region MessageHandler override Methods
        /// <inheritdoc />
        public override Task Handle(AddrMessage message, IPeer sender)
        {
            var connectedEndPoints = _serverContext.ConnectedPeers.Keys.ToArray();

            var endPointsToConnect = message.Payload.Address
                .Select(nat => new EndPoint(Protocol.Tcp, nat.EndPoint))
                .Where(ep => !connectedEndPoints.Contains(ep))
                .ToArray();

            _server.ConnectToPeers(endPointsToConnect);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override bool CanHandle(Message message)
        {
            return message is AddrMessage;
        }
        #endregion
    }
}