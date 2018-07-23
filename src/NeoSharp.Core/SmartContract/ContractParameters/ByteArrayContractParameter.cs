using NeoSharp.VM;

namespace NeoSharp.Core.SmartContract.ContractParameters
{
    public class ByteArrayContractParameter : ContractParameter
    {
        public ByteArrayContractParameter(byte[] value) : base(ContractParameterType.ByteArray, value) { }

        public override void PushIntoScriptBuilder(ScriptBuilder scriptBuilder)
        {
            scriptBuilder.EmitPush((byte[])this.Value);
        }
    }
}
