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

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public GetAddrMessageMessageHandler()
        {

        }

        public async Task Handle(GetAddrMessage message, IPeer sender)
        {
            // TODO: INJECT IServer

            /*
            var peers = _context.ConnectedPeers.Where(u => u is TcpPeer).Cast<TcpPeer>().ToArray();

            if (peers.Length > MaxCountToSend)
            {
                Random rand = new Random(Environment.TickCount);
                peers = peers.OrderBy(p => rand.Next()).Take(MaxCountToSend).ToArray();
            }

            await sender.Send
                (
                new AddrMessage(peers.Select(p => new NetworkAddressWithTime()
                {
                    EndPoint = p.IPEndPoint,
                    Services = p.Version.Services,
                    Timestamp = p.Version.Timestamp
                }
                )
                .ToArray())
                );
            */
        }
    }
}