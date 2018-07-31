using NeoSharp.Application.Attributes;
using NeoSharp.Application.Client;
using NeoSharp.Core.Network;

namespace NeoSharp.Application.Controllers
{
    public class PromptNetworkController : IPromptController
    {
        #region Private fields

        private readonly IServer _server;
        private readonly INetworkManager _networkManager;
        private readonly IConsoleWriter _consoleWriter;
        private readonly IConsoleReader _consoleReader;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="networkManager">Network manages</param>
        /// <param name="consoleWriter">Console writter</param>
        /// <param name="consoleReader">Console reader</param>
        public PromptNetworkController(IServer server, INetworkManager networkManager, IConsoleWriter consoleWriter, IConsoleReader consoleReader)
        {
            _server = server;
            _networkManager = networkManager;
            _consoleReader = consoleReader;
            _consoleWriter = consoleWriter;
        }

        /// <summary>
        /// Nodes
        /// </summary>
        [PromptCommand("nodes", Category = "Network", Help = "Get nodes information")]
        public void NodesCommand()
        {
            var peers = _server.ConnectedPeers;

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