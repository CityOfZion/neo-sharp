using NeoSharp.Types;
using NeoSharp.VM;

namespace NeoSharp.Core.SmartContract.ContractParameters
{
    public class Hash256ContractParameter : ContractParameter
    {
        public Hash256ContractParameter(UInt256 value) : base(ContractParameterType.Hash256, value) { }

        public override void PushIntoScriptBuilder(ScriptBuilder scriptBuilder)
        {
            var valueAsUInt256 = Value as UInt256;
            scriptBuilder.EmitPush(valueAsUInt256.ToArray());
        }
    }
}
