using System;
using System.Threading;
using System.Threading.Tasks;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Helpers;
using NeoSharp.Core.Messaging;
using NeoSharp.Core.Messaging.Messages;

namespace NeoSharp.Core.Network
{
    public class PeerMessageListener : IPeerMessageListener
    {
        #region Private Fields

        private static readonly TimeSpan DefaultMessagePollingInterval = TimeSpan.FromMilliseconds(100);

        private readonly IAsyncDelayer _asyncDelayer;
        private readonly IMessageHandler<Message> _messageHandler;
        private readonly IServerContext _serverContext;

        #endregion

        #region Constructor

        public PeerMessageListener(
            IAsyncDelayer asyncDelayer,
            IMessageHandler<Message> messageHandler,
            IServerContext serverContext)
        {
            _asyncDelayer = asyncDelayer ?? throw new ArgumentNullException(nameof(asyncDelayer));
            _messageHandler = messageHandler ?? throw new ArgumentNullException(nameof(messageHandler));
            _serverContext = serverContext ?? throw new ArgumentNullException(nameof(serverContext));
        }

        #endregion

        #region IPeerMessageListener implementation 

        public void StartFor(IPeer peer, CancellationToken cancellationToken)
        {
            // Initiate handshake
            peer.Send(new VersionMessage(_serverContext.Version));

            Task.Factory.StartNew(async () =>
            {
                while (peer.IsConnected)
                {
                    var message = await peer.Receive();
                    if (message == null)
                    {
                        await _asyncDelayer.Delay(DefaultMessagePollingInterval, cancellationToken);
                        continue;
                    }

                    // TODO #369: Peer that sending wrong messages has to be disconnected.
                    if (peer.IsReady == message.IsHandshakeMessage()) continue;

                    await _messageHandler.Handle(message, peer);
                }
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        #endregion
    }
}
