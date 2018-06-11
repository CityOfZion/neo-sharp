using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class AddrMessageMessageHandler : IMessageHandler<AddrMessage>
    {
        #region Variables

        private readonly IServerContext _context;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Context</param>
        public AddrMessageMessageHandler(IServerContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task Handle(AddrMessage message, IPeer sender)
        {
            // TODO: do logic

            // IPEndPoint[] peers = message.Payload.Address.Select(p => p.EndPoint).Where(p => p.Port != localNode.Port || !LocalNode.LocalAddresses.Contains(p.Address)).ToArray();
            // if (peers.Length > 0) PeersReceived?.Invoke(this, peers);

            await Task.CompletedTask;
        }
    }
}