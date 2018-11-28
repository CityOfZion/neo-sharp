using System.Collections.Generic;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.SmartContract.ContractParameters;
using NeoSharp.Types;
using NeoSharp.VM;

namespace NeoSharp.Core.Wallet.Invocation
{
    public class InvocationProcess
    {
        private readonly ExecutionEngineBase _executionEngine;

        public InvocationProcess(ExecutionEngineBase executionEngine)
        {
            _executionEngine = executionEngine;
        }

        /// <summary>
        /// Invokes the specified Contract using the provided parameters.
        /// </summary>
        /// <returns>The invoke.</returns>
        /// <param name="scriptHash">Script hash.</param>
        /// <param name="contractParameters">Contract parameters.</param>
        public InvocationResult Invoke(UInt160 scriptHash, ContractParameter[] contractParameters)
        {
            var invocationScript = BuildInvocationScript(scriptHash, contractParameters);
            var invocationResult = RunInvocationOnVM(invocationScript);
            return invocationResult;
        }

        /// <summary>
        /// Runs the script on the VM.
        /// </summary>
        /// <returns>The invocation script on vm.</returns>
        /// <param name="invocationScript">Invocation script.</param>
        private InvocationResult RunInvocationOnVM(byte[] invocationScript)
        {
            _executionEngine.LoadScript(invocationScript);
            _executionEngine.Execute();

            var resultList = new List<object>();
            foreach (var stackItem in _executionEngine.ResultStack)
            {
                using (stackItem)
                {
                    resultList.Add(stackItem.ToObject());
                }
            }

            var invocationResult = new InvocationResult(invocationScript, _executionEngine.State,
                                                        _executionEngine.ConsumedGas, resultList.ToArray());
            return invocationResult;
        }

        /// <summary>
        /// Builds the invocation script.
        /// </summary>
        /// <returns>The invocation script.</returns>
        /// <param name="scriptHash">Script hash.</param>
        /// <param name="parameters">Parameters.</param>
        private byte[] BuildInvocationScript(UInt160 scriptHash, ContractParameter[] parameters)
        {
            byte[] executionScript;

            using (var sb = new ScriptBuilder())
            {
                for (var i = parameters.Length - 1; i >= 0; i--)
                {
                    sb.PushContractParameter(parameters[i]);
                }
                sb.EmitAppCall(scriptHash.ToArray());
                executionScript = sb.ToArray();
            }

            return executionScript;
        }
    }
}
