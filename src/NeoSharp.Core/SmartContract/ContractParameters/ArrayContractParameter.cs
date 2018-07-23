using NeoSharp.VM;

namespace NeoSharp.Core.SmartContract.ContractParameters
{
    public class ArrayContractParameter : ContractParameter
    {
        public ArrayContractParameter(ContractParameter[] value) : base(ContractParameterType.Array, value) { }

        public override void PushIntoScriptBuilder(ScriptBuilder scriptBuilder)
        {
            scriptBuilder.EmitPush((object[])Value);
        }
    }
}
