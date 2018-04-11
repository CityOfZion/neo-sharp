using Microsoft.Extensions.Logging;

namespace NeoSharp.Core.Network
{
    public class NetworkManager : INetworkManager
    {
        private readonly IServer _server;        

        public NetworkManager(ILogger<NetworkManager> logger, IServer serverInit)
        {
            this._server = serverInit;

            logger.LogInformation("Network Manager Initialised");
        }

        public void StartNetwork()
        {            
            this._server.StartServer();            
        }

        public void StopNetwork()
        {           
            this._server.StopServer();
        }
    }
}
