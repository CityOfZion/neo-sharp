using Newtonsoft.Json;

namespace NeoSharp.VM.TestHelper
{
    public class VMUTStep
    {
        [JsonProperty]
        public string Comment { get; set; }

        [JsonProperty]
        public VMUTActionType[] Actions { get; set; }

        [JsonProperty]
        public VMUTExecutionEngineState State { get; set; }
    }
}