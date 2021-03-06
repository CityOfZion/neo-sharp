﻿namespace NeoSharp.VM.NeoVM
{
    public class ExecutionContextWrapper : ExecutionContextBase
    {
        internal Neo.VM.ExecutionContext NativeContext { get; }

        public override Stack EvaluationStack { get; }

        public override Stack AltStack { get; }

        public override byte[] ScriptHash => NativeContext.ScriptHash;

        public override EVMOpCode NextInstruction
        {
            get
            {
                try
                {
                    // TODO: Remove this try when NeoVM push a new version

                    return (EVMOpCode)NativeContext.NextInstruction;
                }
                catch { return EVMOpCode.RET; }
            }
        }

        public override int InstructionPointer => NativeContext.InstructionPointer;

        public ExecutionContextWrapper(Neo.VM.ExecutionContext context)
        {
            NativeContext = context;
            EvaluationStack = new StackWrapper(context.EvaluationStack);
            AltStack = new StackWrapper(context.AltStack);
        }
    }
}