using NeoSharp.VM;

namespace NeoSharp.Core.SmartContract.ContractParameters
{
    public class BooleanContractParameter : ContractParameter
    {
        public BooleanContractParameter(bool value) : base(ContractParameterType.Boolean, value) { }

        public override void PushIntoScriptBuilder(ScriptBuilder scriptBuilder)
        {
            scriptBuilder.EmitPush((bool)this.Value);
        }
    }
}
