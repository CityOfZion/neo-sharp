using System;
using System.Linq;
using NeoSharp.Core.Models;
using NeoSharp.VM;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Cryptography;
using NeoSharp.Types.ExtensionMethods;

namespace NeoSharp.Core.SmartContract
{
    public class ContractFactory
    {
        /// <summary>
        /// Creates the single public key redeem contract ('regular account')
        /// </summary>
        /// <returns>The single public key redeem contract.</returns>
        /// <param name="publicKey">Public key.</param>
        public static Contract CreateSinglePublicKeyRedeemContract(ECPoint publicKey)
        {
            string contractHexCode;
            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitPush(publicKey.EncodedData);
                sb.Emit(EVMOpCode.CHECKSIG);
                contractHexCode = sb.ToArray().ToHexString();
            }

            ContractParameterType returnType = ContractParameterType.Void; 
            ContractParameterType[] parameters = { ContractParameterType.Signature };

            Code contractCode = new Code {
                Script = contractHexCode.HexToBytes(),
                ScriptHash = contractHexCode.HexToBytes().ToScriptHash(),
                ReturnType = returnType,
                Parameters = parameters
            };

            Contract contract = new Contract
            {
                Code = contractCode
            };

            return contract;
        }

        /// <summary>
        /// Creates the multiple public key redeem contract ('multisig account')
        /// </summary>
        /// <returns>The multiple public key redeem contract.</returns>
        /// <param name="numberOfRequiredPublicKeys">Number of required public keys.</param>
        /// <param name="publicKeys">Public keys.</param>
        public static Contract CreateMultiplePublicKeyRedeemContract(int numberOfRequiredPublicKeys, ECPoint[] publicKeys)
        {
            if (!((1 <= numberOfRequiredPublicKeys) 
                  && (numberOfRequiredPublicKeys <= publicKeys.Length) 
                  && (publicKeys.Length <= 1024)))
                throw new ArgumentException("Invalid public keys. ");

            byte[] contractHexCode;

            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitPush(numberOfRequiredPublicKeys);
                foreach (ECPoint publicKey in publicKeys.OrderBy(p => p))
                {
                    sb.EmitPush(publicKey.EncodedData);
                }
                sb.EmitPush(publicKeys.Length);
                sb.Emit(EVMOpCode.CHECKMULTISIG);
                contractHexCode = sb.ToArray();
            }

            ContractParameterType returnType = ContractParameterType.Void; 
            ContractParameterType[] parameters = Enumerable.Repeat(ContractParameterType.Signature, numberOfRequiredPublicKeys).ToArray();

            Code contractCode = new Code
            {
                Script = contractHexCode,
                ScriptHash = contractHexCode.ToScriptHash(),
                ReturnType = returnType,
                Parameters = parameters
            };

            Contract contract = new Contract
            {
                Code = contractCode
            };

            return contract;
        }
    }
}