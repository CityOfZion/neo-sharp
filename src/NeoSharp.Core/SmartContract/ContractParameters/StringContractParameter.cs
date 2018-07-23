using NeoSharp.VM;

namespace NeoSharp.Core.SmartContract.ContractParameters
{
    public class StringContractParameter : ContractParameter
    {
        public StringContractParameter(string value) : base(ContractParameterType.String, value) { }

        public override void PushIntoScriptBuilder(ScriptBuilder scriptBuilder)
        {
            scriptBuilder.EmitPush((string)Value);
        }
    }
}
