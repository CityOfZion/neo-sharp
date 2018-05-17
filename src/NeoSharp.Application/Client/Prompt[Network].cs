using NeoSharp.Application.Attributes;
using NeoSharp.Core.Network;
using System.Linq;

namespace NeoSharp.Application.Client
{
    public partial class Prompt : IPrompt
    {
        /// <summary>
        /// Nodes
        /// </summary>
        [PromptCommand("nodes", Category = "Network", Help = "Get nodes information")]
        // ReSharper disable once UnusedMember.Local
        private void NodesCommand()
        {
            lock (_server.ConnectedPeers)
            {
                IPeer[] peers = _server.ConnectedPeers.ToArray();

                _consoleWriter.WriteLine("Connected: " + peers.Length.ToString());

                foreach (IPeer p in peers)
                {
                    _consoleWriter.WriteLine(p.ToString());
                }
            }
        }

        /// <summary>
        /// Start rpc
        /// </summary>
        [PromptCommand("rpc start", Category = "Network")]
        private void StartRpcCommand()
        {
            _rpc?.Start();
        }

        /// <summary>
        /// Stop rpc
        /// </summary>
        [PromptCommand("rpc stop", Category = "Network")]
        private void StopRpcCommand()
        {
            _rpc?.Stop();
        }

        /// <summary>
        /// Start network
        /// </summary>
        [PromptCommand("network start", Category = "Network")]
        // ReSharper disable once UnusedMember.Local
        private void StartCommand()
        {
            _networkManager?.StartNetwork();
        }

        /// <summary>
        /// Stop network
        /// </summary>
        [PromptCommand("network stop", Category = "Network")]
        private void StopCommand()
        {
            _networkManager?.StopNetwork();
        }
    }
}