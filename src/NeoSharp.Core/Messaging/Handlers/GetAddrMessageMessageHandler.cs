using System;
using System.Linq;
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

        private readonly IServer _server;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public GetAddrMessageMessageHandler(IServer server)
        {
            _server = server ?? throw new ArgumentNullException(nameof(server));
        }

        public async Task Handle(GetAddrMessage message, IPeer sender)
        {
            var rand = new Random(Environment.TickCount);
            var peers = _server.ConnectedPeers
                .OrderBy(p => rand.Next())
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
    }
}