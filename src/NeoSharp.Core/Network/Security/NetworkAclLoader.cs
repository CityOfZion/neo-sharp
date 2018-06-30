using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace NeoSharp.Core.Network.Security
{
    public class NetworkAclLoader : INetworkAclLoader
    {
        /// <inheritdoc />
        public NetworkAcl Load(NetworkAclConfig config)
        {
            if (config == null) return null;

            var entries = new NetworkAcl.Entry[0];

            if (!string.IsNullOrEmpty(config.Path) && File.Exists(config.Path))
            {
                var json = File.ReadAllText(config.Path);
                var jToken = JToken.Parse(json);

                if (jToken is JArray jArray)
                {
                    entries = jArray
                        .Where(it => it["value"] != null)
                        .Select(it =>
                        {
                            var value = (string)it["value"];

                            if ((bool?)it["regex"] ?? false)
                                return new NetworkAcl.RegexEntry(value);

                            return new NetworkAcl.Entry(value);

                        })
                        .ToArray();
                }
            }

            return new NetworkAcl(config.Type, entries);
        }
    }
}