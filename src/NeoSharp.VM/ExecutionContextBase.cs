using NeoSharp.VM.Extensions;

namespace NeoSharp.VM
{
    public abstract class ExecutionContextBase
    {
        /// <summary>
        /// ScriptHashLength
        /// </summary>
        public const int ScriptHashLength = 20;

        /// <summary>
        /// Evaluation Stack
        /// </summary>
        public abstract Stack EvaluationStack { get; }

        /// <summary>
        /// Alt Stack
        /// </summary>
        public abstract Stack AltStack { get; }

        /// <summary>
        /// Script Hash
        /// </summary>
        public abstract byte[] ScriptHash { get; }

        /// <summary>
        /// Next instruction
        /// </summary>
        public abstract EVMOpCode NextInstruction { get; }

        /// <summary>
        /// Get Instruction pointer
        /// </summary>
        public abstract int InstructionPointer { get; }

        /// <summary>
        /// String representation
        /// </summary>
        public override string ToString()
        {
            return $"[{ScriptHash.ToHexString()}-{InstructionPointer:x6}] {NextInstruction}";
        }
    }
}