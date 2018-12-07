using System.Collections.Generic;
using Newtonsoft.Json;

namespace NeoSharp.VM.TestHelper
{
    public class VMUTEntry
    {
        public class ScriptEntry
        {
            [JsonProperty]
            public byte[] Script { get; set; }

            [JsonProperty]
            public bool HasDynamicInvoke { get; set; }

            [JsonProperty]
            public bool HasStorage { get; set; }

            [JsonProperty]
            public bool Payable { get; set; }
        }

        [JsonProperty]
        public IList<ScriptEntry> ScriptTable { get; set; }

        [JsonProperty]
        public byte[] Script { get; set; }

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public VMUTStep[] Steps { get; set; }

        [JsonProperty]
        public ETriggerType Trigger { get; set; }
    }
}