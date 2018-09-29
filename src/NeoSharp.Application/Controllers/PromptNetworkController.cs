using NeoSharp.Application.Attributes;
using NeoSharp.Application.Client;
using NeoSharp.Core.Network;

namespace NeoSharp.Application.Controllers
{
    public class PromptNetworkController : IPromptController
    {
        #region Private fields
        private readonly IServerContext _serverContext;
        private readonly INetworkManager _networkManager;
        private readonly IConsoleWriter _consoleWriter;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverContext">Server context</param>
        /// <param name="networkManager">Network manages</param>
        /// <param name="consoleWriter">Console writter</param>
        public PromptNetworkController(IServerContext serverContext, INetworkManager networkManager, IConsoleWriter consoleWriter)
        {
            _serverContext = serverContext;
            _networkManager = networkManager;
            _consoleWriter = consoleWriter;
        }

        /// <summary>
        /// Nodes
        /// </summary>
        [PromptCommand("nodes", Category = "Network", Help = "Get nodes information")]
        public void NodesCommand()
        {
            var peers = this._serverContext.ConnectedPeers;

            _consoleWriter.WriteLine("Connected: " + peers.Count);

            foreach (var peer in peers)
            {
                _consoleWriter.WriteLine(peer.ToString());
            }
        }

        /// <summary>
        /// Start network
        /// </summary>
        [PromptCommand("network start", Category = "Network")]
        // ReSharper disable once UnusedMember.Local
        public void NetworkStartCommand()
        {
            _networkManager?.StartNetwork();
        }

        /// <summary>
        /// Stop network
        /// </summary>
        [PromptCommand("network stop", Category = "Network")]
        public void NetworkStopCommand()
        {
            _networkManager?.StopNetwork();
        }
    }
}