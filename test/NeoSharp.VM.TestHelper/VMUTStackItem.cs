using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NeoSharp.VM.TestHelper
{
    public class VMUTStackItem
    {
        [JsonProperty]
        public VMUTStackItemType Type { get; set; }

        [JsonProperty]
        public JToken Value { get; set; }
    }
}