using NeoSharp.VM;

namespace NeoSharp.Core.SmartContract.ContractParameters
{
    public class SignatureContractParameter : ContractParameter
    {
        public SignatureContractParameter(byte[] value) : base(ContractParameterType.Signature, value) { }

        public override void PushIntoScriptBuilder(ScriptBuilder scriptBuilder)
        {
            scriptBuilder.EmitPush((byte[])Value);
        }
    }
}
