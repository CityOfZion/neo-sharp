using System;
using System.IO;
using NeoSharp.VM.Types;

namespace NeoSharp.VM.Attributes
{
    public class InstructionAttribute : Attribute
    {
        /// <summary>
        /// Instruction type
        /// </summary>
        public Type InstructionType { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public InstructionAttribute()
        {
            InstructionType = typeof(Instruction);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="instructionType">Instruction type</param>
        protected InstructionAttribute(Type instructionType)
        {
            InstructionType = instructionType;
        }

        /// <summary>
        /// Create new instruction
        /// </summary>
        /// <param name="location">Location</param>
        /// <param name="opCode">Opcode</param>
        public Instruction New(InstructionLocation location, EVMOpCode opCode)
        {
            var ret = (Instruction)Activator.CreateInstance(InstructionType);

            ret.Location = location;
            ret.OpCode = opCode;

            return ret;
        }

        /// <summary>
        /// Fill instruction
        /// </summary>
        /// <param name="reader">Reader</param>
        /// <param name="instruction">Instruction</param>
        /// <returns>True if is filled</returns>
        public virtual bool Fill(BinaryReader reader, Instruction instruction)
        {
            return true;
        }
    }
}