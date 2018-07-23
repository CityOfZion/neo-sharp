using System.Numerics;
using NeoSharp.VM;

namespace NeoSharp.Core.SmartContract.ContractParameters
{
    public class IntegerContractParameter : ContractParameter
    {
        public IntegerContractParameter(int value) : base(ContractParameterType.Integer, value) { }
        public IntegerContractParameter(short value) : base(ContractParameterType.Integer, value) { }
        public IntegerContractParameter(long value) : base(ContractParameterType.Integer, value) { }
        public IntegerContractParameter(byte value) : base(ContractParameterType.Integer, value) { }
        public IntegerContractParameter(uint value) : base(ContractParameterType.Integer, value) { }
        public IntegerContractParameter(ushort value) : base(ContractParameterType.Integer, value) { }
        public IntegerContractParameter(ulong value) : base(ContractParameterType.Integer, value) { }
        public IntegerContractParameter(sbyte value) : base(ContractParameterType.Integer, value) { }
        public IntegerContractParameter(BigInteger value) : base(ContractParameterType.Integer, value) { }

        public override void PushIntoScriptBuilder(ScriptBuilder scriptBuilder)
        {
            if (this.Value is BigInteger bi)
            {
                scriptBuilder.EmitPush(bi);
            }
            else
            {
                if (BigInteger.TryParse(this.Value.ToString(), out var bip))
                {
                    scriptBuilder.EmitPush(bip);
                }
            }
        }
    }
}
