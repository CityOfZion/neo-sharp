using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NeoSharp.Application.Attributes;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Models;
using NeoSharp.Core.Types;
using Newtonsoft.Json.Linq;

namespace NeoSharp.Application.Client
{
    public partial class Prompt : IPrompt
    {
        /// <summary>
        /// Make rpc call
        /// </summary>
        private Task RpcCallCommand(IPEndPoint endPoint, string method, string parameters = null)
        {
            return RpcCallCommand<object>(endPoint, method, parameters, false);
        }

        /// <summary>
        /// Make rpc call
        /// </summary>
        private async Task RpcCallCommand<T>(IPEndPoint endPoint, string method, string parameters = null, bool deserializeResult = false)
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

                var json = JObject.Parse(await rest.Content.ReadAsStringAsync());

                if (deserializeResult)
                {
                    var obj = BinaryDeserializer.Default.Deserialize<T>(json["result"].Value<string>().HexToBytes());

                    _consoleWriter.WriteObject(obj, PromptOutputStyle.json);
                }
                else
                {
                    _consoleWriter.WriteObject(json, PromptOutputStyle.json);
                }
            }
        }

        /// <summary>
        /// Start rpc
        /// </summary>
        [PromptCommand("rpc start", Category = "Rpc")]
        private void RpcStartCommand()
        {
            _rpc?.Start();
        }

        /// <summary>
        /// Stop rpc
        /// </summary>
        [PromptCommand("rpc stop", Category = "Rpc")]
        private void RpcStopCommand()
        {
            _rpc?.Stop();
        }

        /// <summary> 
        /// Make rpc call for `getrawmempool` 
        /// </summary> 
        [PromptCommand("rpc getrawmempool", Category = "Rpc", Help = "Make rpc calls for memorypool")]
        private Task RpcGetrawmempoolCommand(IPEndPoint endPoint)
        {
            return RpcCallCommand(endPoint, "getrawmempool", null);
        }

        /// <summary> 
        /// Make rpc call for `getvalidators` 
        /// </summary> 
        [PromptCommand("rpc getvalidators", Category = "Rpc", Help = "Make rpc calls for getvalidators")]
        private Task RpcGetvalidatorsCommand(IPEndPoint endPoint)
        {
            return RpcCallCommand(endPoint, "getvalidators", null);
        }

        /// <summary> 
        /// Make rpc call for `invokescript` 
        /// </summary> 
        [PromptCommand("rpc invokescript", Category = "Rpc", Help = "Make rpc call for invokescript")]
        private Task RpcInvokescriptCommand(IPEndPoint endPoint, byte[] script)
        {
            return RpcCallCommand(endPoint, "invokescript", "[\"" + script.ToHexString(false) + "\"]");
        }

        /// <summary> 
        /// Make rpc call for `getpeers` 
        /// </summary> 
        [PromptCommand("rpc getpeers", Category = "Rpc", Help = "Make rpc calls for getpeers")]
        private Task RpcGetpeersCommand(IPEndPoint endPoint)
        {
            return RpcCallCommand(endPoint, "getpeers", null);
        }

        /// <summary> 
        /// Make rpc call for `getversion` 
        /// </summary> 
        [PromptCommand("rpc getversion", Category = "Rpc", Help = "Make rpc calls for getversion")]
        private Task RpcGetversionCommand(IPEndPoint endPoint)
        {
            return RpcCallCommand(endPoint, "getversion", null);
        }

        /// <summary> 
        /// Make rpc call for `validateaddress` 
        /// </summary> 
        [PromptCommand("rpc validateaddress", Category = "Rpc", Help = "Make rpc calls for validateaddress")]
        private Task RpcValidateaddressCommand(IPEndPoint endPoint, string address)
        {
            return RpcCallCommand(endPoint, "validateaddress", "[\"" + address + "\"]");
        }

        /// <summary> 
        /// Make rpc call for `getconnectioncount` 
        /// </summary> 
        [PromptCommand("rpc getconnectioncount", Category = "Rpc", Help = "Make rpc calls for getconnectioncount")]
        private Task RpcGetconnectioncountCommand(IPEndPoint endPoint)
        {
            return RpcCallCommand(endPoint, "getconnectioncount", null);
        }

        /// <summary> 
        /// Make rpc call for `getblocksysfee` 
        /// </summary> 
        [PromptCommand("rpc getblocksysfee", Category = "Rpc", Help = "Make rpc calls for getblocksysfee")]
        private Task RpcGetblocksysfeeCommand(IPEndPoint endPoint, uint height)
        {
            return RpcCallCommand(endPoint, "getblocksysfee", "[" + height.ToString() + "]");
        }

        /// <summary> 
        /// Make rpc call for `getbestblockhash` 
        /// </summary> 
        [PromptCommand("rpc getbestblockhash", Category = "Rpc", Help = "Make rpc calls for getbestblockhash")]
        private Task RpcGetassetstateCommand(IPEndPoint endPoint)
        {
            return RpcCallCommand(endPoint, "getbestblockhash", null);
        }

        /// <summary> 
        /// Make rpc call for `getblockhash` 
        /// </summary> 
        [PromptCommand("rpc getblockhash", Category = "Rpc", Help = "Make rpc calls for getblockhash")]
        private Task RpcGetblockhashCommand(IPEndPoint endPoint, uint height)
        {
            return RpcCallCommand(endPoint, "getblockhash", "[" + height.ToString() + "]");
        }

        /// <summary> 
        /// Make rpc call for `getblock` 
        /// </summary> 
        [PromptCommand("rpc getblock", Category = "Rpc", Help = "Make rpc calls for getblock")]
        private Task RpcGetblockCommand(IPEndPoint endPoint, uint height, bool deserializeResult = false)
        {
            return RpcCallCommand<Block>(endPoint, "getblock", "[" + height.ToString() + "]", deserializeResult);
        }

        /// <summary> 
        /// Make rpc call for `getcontractstate` 
        /// </summary> 
        [PromptCommand("rpc getcontractstate", Category = "Rpc", Help = "Make rpc calls for getcontractstate")]
        private Task RpcGetcontractstateCommand(IPEndPoint endPoint, UInt160 address)
        {
            return RpcCallCommand(endPoint, "getcontractstate", "[\"" + address.ToString(false) + "\"]");
        }

        /// <summary> 
        /// Make rpc call for `getaccountstate` 
        /// </summary> 
        [PromptCommand("rpc getaccountstate", Category = "Rpc", Help = "Make rpc calls for getaccountstate")]
        private Task RpcGetaccountstateCommand(IPEndPoint endPoint, UInt160 address)
        {
            return RpcCallCommand(endPoint, "getaccountstate", "[\"" + address.ToString(false) + "\"]");
        }

        /// <summary> 
        /// Make rpc call for `getassetstate` 
        /// </summary> 
        [PromptCommand("rpc getassetstate", Category = "Rpc", Help = "Make rpc calls for getassetstate")]
        private Task RpcGetassetstateCommand(IPEndPoint endPoint, UInt256 address)
        {
            return RpcCallCommand(endPoint, "getassetstate", "[\"" + address.ToString(false) + "\"]");
        }
    }
}