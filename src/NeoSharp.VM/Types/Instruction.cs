﻿namespace NeoSharp.VM
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
            return $"[{Location}] {OpCode}";
        }

        /// <summary>
        /// To byte array
        /// </summary>
        public virtual byte[] ToByteArray()
        {
            return new byte[] { (byte)OpCode };
        }
    }
}