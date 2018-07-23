using System;
using NeoSharp.Core.Types;
using NeoSharp.VM;

namespace NeoSharp.Core.SmartContract
{
    public class InvocationResult
    {
        public readonly byte[] ExecutedScript;
        public readonly EVMState ResultingState;
        public readonly ulong GasCost;
        public readonly object ExecutionResult;

        public InvocationResult(byte[] executedScript, EVMState resultingState, ulong gasCost, object[] executionResult)
        {
            ExecutedScript = executedScript;
            ResultingState = resultingState;
            GasCost = gasCost;
            ExecutionResult = executionResult;
        }
    }
}
