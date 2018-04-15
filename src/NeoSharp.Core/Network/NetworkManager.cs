using Microsoft.Extensions.Logging;

namespace NeoSharp.Core.Network
{
    public class NetworkManager : INetworkManager
    {
        private readonly IServer _server;        

        public NetworkManager(ILogger<NetworkManager> logger, IServer serverInit)
        {
            _server = serverInit;

            logger.LogInformation("Network Manager Initialised");
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
