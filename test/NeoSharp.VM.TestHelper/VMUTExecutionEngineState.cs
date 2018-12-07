using Newtonsoft.Json;

namespace NeoSharp.VM.TestHelper
{
    public class VMUTExecutionEngineState
    {
        [JsonProperty]
        public VMUTState State { get; set; }

        [JsonProperty]
        public ulong ConsumedGas { get; set; }

        [JsonProperty]
        public string[] Logs { get; set; }

        [JsonProperty]
        public VMUTStackItem[] Notifications { get; set; }

        [JsonProperty]
        public VMUTStackItem[] ResultStack { get; set; }

        [JsonProperty]
        public VMUTExecutionContextState[] InvocationStack { get; set; }
    }
}