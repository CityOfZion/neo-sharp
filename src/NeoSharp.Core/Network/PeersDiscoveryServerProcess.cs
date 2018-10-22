using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NeoSharp.Core.Helpers;
using NeoSharp.Core.Messaging.Messages;

namespace NeoSharp.Core.Network
{
    public class PeersDiscoveryServerProcess : IServerProcess, IDisposable
    {
        private static readonly TimeSpan DefaultProcessStartingDelay = TimeSpan.FromMilliseconds(10_000);
        private static readonly TimeSpan DefaultPeerWaitingInterval = TimeSpan.FromMilliseconds(2_000);
        private static readonly TimeSpan DefaultPeerConnectivityInterval = TimeSpan.FromMilliseconds(5_000);

        private readonly IServerContext _serverContext;
        private readonly IAsyncDelayer _asyncDelayer;
        private CancellationTokenSource _tokenSource;

        public PeersDiscoveryServerProcess(
            IServerContext serverContext,
            IAsyncDelayer asyncDelayer)
        {
            _serverContext = serverContext ?? throw new ArgumentNullException(nameof(serverContext));
            _asyncDelayer = asyncDelayer ?? throw new ArgumentNullException(nameof(asyncDelayer));
        }

        public void Start()
        {
            Stop();

            _tokenSource = new CancellationTokenSource();
            var cancellationToken = _tokenSource.Token;

            Task.Factory.StartNew(async () =>
            {
                await _asyncDelayer.Delay(DefaultProcessStartingDelay, cancellationToken);

                while (!cancellationToken.IsCancellationRequested)
                {
                    if (_serverContext.ConnectedPeers.Count == 0)
                    {
                        await _asyncDelayer.Delay(DefaultPeerWaitingInterval, cancellationToken);
                        continue;
                    }

                    if (_serverContext.ConnectedPeers.Count < _serverContext.MaxConnectedPeers)
                    {
                        var connectedPeers = _serverContext.ConnectedPeers.Values
                            .Where(p => p.IsConnected)
                            .ToArray();

                        try
                        {
                            Parallel.ForEach(connectedPeers, async peer => await peer.Send<GetAddrMessage>());
                        }
                        catch
                        {
                            // ignored
                        }
                    }

                    await _asyncDelayer.Delay(DefaultPeerConnectivityInterval, cancellationToken);
                }
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public void Stop()
        {
            _tokenSource?.Cancel();
            _tokenSource?.Dispose();
        }

        public void Dispose()
        {
            Stop();
        }
    }
}