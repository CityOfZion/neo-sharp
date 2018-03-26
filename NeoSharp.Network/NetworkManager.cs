using Microsoft.Extensions.Logging;

namespace NeoSharp.Network
{
    public class NetworkManager : INetworkManager
    {
        private readonly ILogger<NetworkManager> _logger;        
        private readonly IServer _server;        

        public NetworkManager(ILoggerFactory loggingFactory, IServer serverInit)
        {
            _logger = loggingFactory.CreateLogger<NetworkManager>();            
            _server = serverInit;

            _logger.LogInformation("Network Manager Initialised");
        }

        public void StartNetwork()
        {            
            _server.StartServer();            
        }

        public void StopNetwork()
        {           
            _server.StopServer();
        }
    }
}
