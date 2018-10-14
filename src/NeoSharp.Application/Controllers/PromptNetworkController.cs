using System;
using System.Linq;
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
        private readonly IConsoleHandler _consoleHandler;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverContext">Server context</param>
        /// <param name="networkManager">Network manages</param>
        /// <param name="consoleHandler">Console writter</param>
        public PromptNetworkController(IServerContext serverContext, INetworkManager networkManager, IConsoleHandler consoleHandler)
        {
            _serverContext = serverContext;
            _networkManager = networkManager;
            _consoleHandler = consoleHandler;
        }

        /// <summary>
        /// Nodes
        /// </summary>
        /// <param name="output">Output format</param>
        [PromptCommand("nodes", Category = "Network", Help = "Get nodes information")]
        public void NodesCommand(PromptOutputStyle output = PromptOutputStyle.json)
        {
            var peers = _serverContext.ConnectedPeers;

            switch (output)
            {
                case PromptOutputStyle.json:
                    {
                        _consoleHandler.WriteObject(
                            new
                            {
                                Count = peers.Count,
                                Nodes = peers
                                    .OrderBy(u => u.Value.ConnectionDate)
                                    .Select(u => new { Address = u.Key, ConnectedTime = (DateTime.UtcNow - u.Value.ConnectionDate) })
                                    .ToArray()
                            }, PromptOutputStyle.json);
                        break;
                    }
                default:
                    {
                        _consoleHandler.WriteLine("Connected: " + peers.Count);

                        foreach (var peer in peers.OrderBy(u => u.Value.ConnectionDate))
                        {
                            _consoleHandler.WriteLine(peer.Key.ToString() + " - " +
                                // Connected time
                                (DateTime.UtcNow - peer.Value.ConnectionDate).ToString());
                        }
                        break;
                    }
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
