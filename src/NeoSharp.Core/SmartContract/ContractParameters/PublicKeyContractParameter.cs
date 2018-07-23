using NeoSharp.Core.Cryptography;
using NeoSharp.VM;

namespace NeoSharp.Core.SmartContract.ContractParameters
{
    public class PublicKeyContractParameter : ContractParameter
    {
        public PublicKeyContractParameter(ECPoint value) : base(ContractParameterType.PublicKey, value) { }

        public override void PushIntoScriptBuilder(ScriptBuilder scriptBuilder)
        {
            var valueAsECPoint = Value as ECPoint;
            scriptBuilder.EmitPush(valueAsECPoint.EncodedData);
        }
    }
}
