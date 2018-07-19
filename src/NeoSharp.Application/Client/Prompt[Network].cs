using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NeoSharp.Application.Attributes;

namespace NeoSharp.Application.Client
{
    public partial class Prompt : IPrompt
    {
        /// <summary>
        /// Nodes
        /// </summary>
        [PromptCommand("nodes", Category = "Network", Help = "Get nodes information")]
        private void NodesCommand()
        {
            var peers = _server.ConnectedPeers;

            _consoleWriter.WriteLine("Connected: " + peers.Count);

            foreach (var peer in peers)
            {
                _consoleWriter.WriteLine(peer.ToString());
            }
        }

        /// <summary>
        /// Make rpc call
        /// </summary>
        [PromptCommand("rpc call", Category = "Network", Help = "Make rpc calls to any server")]
        private async Task RpcCallCommand(IPEndPoint endPoint, string method, [PromptCommandParameterBody] string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                parameters = "[]";
            }

            using (HttpClient wb = new HttpClient())
            {
                var content = new StringContent
                    (
                    "{\"jsonrpc\": \"2.0\", \"method\": \"" + method + "\", \"params\": " + parameters + ", \"id\":1}", Encoding.UTF8,
                    "application/json"
                    );

                var rest = await wb.PostAsync("http://" + endPoint.Address.ToString() + ":" + endPoint.Port.ToString(), content);

                if (!rest.IsSuccessStatusCode)
                {
                    _consoleWriter.WriteLine(rest.StatusCode + " - " + rest.ReasonPhrase, ConsoleOutputStyle.Error);
                    return;
                }

                var json = await rest.Content.ReadAsStringAsync();

                _consoleWriter.WriteObject(Newtonsoft.Json.Linq.JObject.Parse(json), PromptOutputStyle.json);
            }
        }

        /// <summary>
        /// Start rpc
        /// </summary>
        [PromptCommand("rpc start", Category = "Network")]
        private void RpcStartCommand()
        {
            _rpc?.Start();
        }

        /// <summary>
        /// Stop rpc
        /// </summary>
        [PromptCommand("rpc stop", Category = "Network")]
        private void RpcStopCommand()
        {
            _rpc?.Stop();
        }

        /// <summary>
        /// Start network
        /// </summary>
        [PromptCommand("network start", Category = "Network")]
        // ReSharper disable once UnusedMember.Local
        private void NetworkStartCommand()
        {
            _networkManager?.StartNetwork();
        }

        /// <summary>
        /// Stop network
        /// </summary>
        [PromptCommand("network stop", Category = "Network")]
        private void NetworkStopCommand()
        {
            _networkManager?.StopNetwork();
        }
    }
}