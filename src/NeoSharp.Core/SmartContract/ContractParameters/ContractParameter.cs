using NeoSharp.VM;

namespace NeoSharp.Core.SmartContract.ContractParameters
{
    public abstract class ContractParameter
    {
        public readonly ContractParameterType Type;
        public readonly object Value;

        protected ContractParameter(ContractParameterType type, object value)
        {
            Type = type;
            Value = value;
        }

        public abstract void PushIntoScriptBuilder(ScriptBuilder scriptBuilder);
    }
}
