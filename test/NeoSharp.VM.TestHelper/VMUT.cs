using Newtonsoft.Json;

namespace NeoSharp.VM.TestHelper
{
    public class VMUT
    {
        [JsonProperty]
        public string Category { get; set; }

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public VMUTEntry[] Tests { get; set; }
    }
}