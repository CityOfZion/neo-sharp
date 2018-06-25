using System;
using System.Net;
using System.Threading.Tasks;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class AddrMessageMessageHandler : IMessageHandler<AddrMessage>
    {
        #region Variables

        private readonly IPeerFactory _factory;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="factory">Factory</param>
        public AddrMessageMessageHandler(IPeerFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task Handle(AddrMessage message, IPeer sender)
        {
            // TODO: INJECT IServer

            IPEndPoint[] peers = null;

            //    message.Payload.Address
            //    .Select(p => p.EndPoint)
            //    .Where(
            //        p =>
            //        //p.Port != _context.ListenEndPoint.Port || // TODO: Check this
            //        !_context.ConnectedPeers
            //        .Where(u => u is TcpPeer)
            //        .Cast<TcpPeer>()
            //        .Any(u => u.IPEndPoint.Equals(p.Address))
            //        )
            //    .ToArray();

            if (peers.Length > 0)
            {
                // TODO: How can we connect here?

                //Parallel.ForEach(peers, async (peer) =>
                //{
                //    var connected = await _factory.ConnectTo(new Network.EndPoint()
                //    {
                //        Host = peer.Address.ToString(),
                //        Port = peer.Port,
                //        Protocol = Protocol.Tcp
                //    });

                //    _context.ConnectedPeers.Add(connected);
                //});
            }

            await Task.CompletedTask;
        }
    }
}