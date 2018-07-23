namespace NeoSharp.VM.Types
{
    public class Instruction
    {
        /// <summary>
        /// Location
        /// </summary>
        public InstructionLocation Location { get; set; }

        /// <summary>
        /// Opcode
        /// </summary>
        public EVMOpCode OpCode { get; set; }

        /// <summary>
        /// String representation
        /// </summary>
        public override string ToString()
        {
            return "[" + Location.ToString() + "] " + OpCode.ToString();
        }
    }
}