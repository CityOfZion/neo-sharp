using Newtonsoft.Json;

namespace NeoSharp.VM.TestHelper
{
    public class VMUTStackItem
    {
        [JsonProperty]
        public VMUTStackItemType Type { get; set; }

        [JsonProperty]
        public string Value { get; set; }
    }
}