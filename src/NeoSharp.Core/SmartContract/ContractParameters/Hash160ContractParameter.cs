using NeoSharp.Types;
using NeoSharp.VM;

namespace NeoSharp.Core.SmartContract.ContractParameters
{
    public class Hash160ContractParameter : ContractParameter
    {
        public Hash160ContractParameter(UInt160 value) : base(ContractParameterType.Hash160, value) { }

        public override void PushIntoScriptBuilder(ScriptBuilder scriptBuilder)
        {
            var valueAsUInt160 = Value as UInt160;
            scriptBuilder.EmitPush(valueAsUInt160.ToArray());
        }
    }
}
