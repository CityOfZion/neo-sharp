using System.IO;
using System.Linq;
using NeoSharp.Core.Types.Json;

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
                var jObject = JObject.Parse(json);

                if (jObject is JArray jArray)
                {
                    entries = jArray
                        .Where(it => it.ContainsProperty("value"))
                        .Select(it =>
                        {
                            var value = it.Properties["value"].AsString();

                            if (it.ContainsProperty("regex") && it.Properties["regex"].AsBooleanOrDefault())
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