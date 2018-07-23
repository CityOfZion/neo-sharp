namespace NeoSharp.VM.Types
{
    public class InstructionWithPayload : Instruction
    {
        /// <summary>
        /// Payload
        /// </summary>
        public long PayloadSize { get; set; }

        /// <summary>
        /// Payload
        /// </summary>
        public byte[] Payload { get; set; }
    }
}