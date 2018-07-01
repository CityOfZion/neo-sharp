using NeoSharp.Core.Logging;

namespace NeoSharp.Core.Network
{
    public class NetworkManager : INetworkManager
    {
        #region Private fields

        private readonly IServer _server;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="serverInit">Server</param>
        public NetworkManager(ILogger<NetworkManager> logger, IServer serverInit)
        {
            _server = serverInit;
            logger.LogInformation("Network Manager Initialised");
        }

        /// <summary>
        /// Start network
        /// </summary>
        public void StartNetwork()
        {
            _server.Start();
        }

        /// <summary>
        /// Stop network
        /// </summary>
        public void StopNetwork()
        {
            _server.Stop();
        }
    }
}