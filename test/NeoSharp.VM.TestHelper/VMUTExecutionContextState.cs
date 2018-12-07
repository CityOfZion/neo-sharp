using Newtonsoft.Json;

namespace NeoSharp.VM.TestHelper
{
    public class VMUTExecutionContextState
    {
        [JsonProperty]
        public byte[] ScriptHash { get; set; }

        [JsonProperty]
        public EVMOpCode NextInstruction { get; set; }

        [JsonProperty]
        public int InstructionPointer { get; set; }

        // Stacks

        [JsonProperty]
        public VMUTStackItem[] AltStack { get; set; }

        [JsonProperty]
        public VMUTStackItem[] EvaluationStack { get; set; }
    }
}