using Microsoft.Extensions.Logging;

namespace NeoSharp.Network
{
    public class PeerFactory : IPeerFactory
    {
        private readonly ILoggerFactory _loggerFactory;

        public PeerFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public IPeer Create()
        {
            return new Peer(_loggerFactory);
        }
    }
}
