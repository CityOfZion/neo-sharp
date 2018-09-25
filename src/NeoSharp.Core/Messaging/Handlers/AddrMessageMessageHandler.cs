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
        #endregion

        #region Constructor
        //public AddrMessageMessageHandler(IServer server)
        //{
        //    _server = server ?? throw new ArgumentNullException(nameof(server));
        //}
        public AddrMessageMessageHandler()
        {
        }
        #endregion

        #region MessageHandler override Methods
        /// <inheritdoc />
        public override Task Handle(AddrMessage message, IPeer sender)
        {
            //var connectedEndPoints = _server.ConnectedPeers
            //    .Select(p => p.EndPoint)
            //    .ToArray();

            //var endPointsToConnect = message.Payload.Address
            //    .Select(nat => new EndPoint(Protocol.Tcp, nat.EndPoint))
            //    .Where(ep => !connectedEndPoints.Contains(ep))
            //    .ToArray();

            //_server.ConnectToPeers(endPointsToConnect);

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