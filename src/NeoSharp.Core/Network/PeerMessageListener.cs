using System;
using System.Threading;
using System.Threading.Tasks;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Helpers;
using NeoSharp.Core.Messaging;

namespace NeoSharp.Core.Network
{
    public class PeerMessageListener : IPeerMessageListener
    {
        #region Private Fields

        private static readonly TimeSpan DefaultMessagePollingInterval = TimeSpan.FromMilliseconds(100);
        private static readonly TimeSpan DefaultReceiveTimeout = TimeSpan.FromMilliseconds(1_000);

        private readonly IAsyncDelayer _asyncDelayer;
        private readonly IMessageHandler<Message> _messageHandler;
        private CancellationTokenSource _messageListenerCancelationToken;

        #endregion

        #region Constructor

        public PeerMessageListener(IAsyncDelayer asyncDelayer, IMessageHandler<Message> messageHandler)
        {
            _asyncDelayer = asyncDelayer;
            _messageHandler = messageHandler;

            _messageListenerCancelationToken = null;
        }

        #endregion

        #region IPeerMessageListener implementation

        public void StartListen(IPeer peer)
        {
            // Create cancel token

            if (_messageListenerCancelationToken != null)
            {
                _messageListenerCancelationToken.Cancel();
            }

            _messageListenerCancelationToken = new CancellationTokenSource(DefaultReceiveTimeout);

            // Start tasks

            Task.Factory.StartNew(async () =>
            {
                while (peer.IsConnected)
                {
                    var message = await peer.Receive();
                    if (message == null)
                    {
                        await _asyncDelayer.Delay(DefaultMessagePollingInterval, _messageListenerCancelationToken.Token);
                        continue;
                    }

                    // TODO: Peer that sending wrong messages has to be disconnected.
                    if (peer.IsReady == message.IsHandshakeMessage()) continue;

                    await _messageHandler.Handle(message, peer);
                }
            },
            _messageListenerCancelationToken.Token,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default
            );
        }

        public void StopListenAllPeers()
        {
            _messageListenerCancelationToken?.Cancel();
        }

        #endregion
    }
}