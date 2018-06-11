using System;
using System.Threading.Tasks;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class GetAddrMessageMessageHandler : IMessageHandler<GetAddrMessage>
    {
        #region Constants

        const int MaxCountToSend = 200;

        #endregion

        #region Variables

        private readonly IServerContext _context;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Context</param>
        public GetAddrMessageMessageHandler(IServerContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task Handle(GetAddrMessage message, IPeer sender)
        {
            /*
            // TODO logic
            var peers = _server.ConnectedPeers.Select(u => u.EndPoint).ToArray();

            if (peers.Count > MaxCountToSend)
            {
                Random rand = new Random(Environment.TickCount);
                peers = peers.OrderBy(p => rand.Next());
                peers = peers.Take(MaxCountToSend);
            }

            await sender.Send(new AddrMessage(peers.Select(p => NetworkAddressWithTime.Create(p.ListenerEndpoint, p.Version.Services, p.Version.Timestamp)).ToArray()));
            */
            await Task.CompletedTask;
        }
    }
}