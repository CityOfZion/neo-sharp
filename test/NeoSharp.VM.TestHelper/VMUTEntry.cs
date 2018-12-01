using Newtonsoft.Json;

namespace NeoSharp.VM.TestHelper
{
    public class VMUTEntry
    {
        [JsonProperty]
        public byte[] Script { get; set; }

        [JsonProperty]
        public VMUTStep[] Steps { get; set; }

        [JsonProperty]
        public VMUTTriggerType Trigger { get; set; }
    }
}