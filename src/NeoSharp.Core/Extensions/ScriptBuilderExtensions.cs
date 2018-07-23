using System;
using NeoSharp.Core.SmartContract.ContractParameters;
using NeoSharp.VM;

namespace NeoSharp.Core.Extensions
{
    public static class ScriptBuilderExtensions
    {
        public static void PushContractParameter(this ScriptBuilder scriptBuilder, ContractParameter parameter)
        {
            parameter.PushIntoScriptBuilder(scriptBuilder);
        }
    }
}
