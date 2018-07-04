using NeoSharp.Core.Models;
using NeoSharp.VM;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.SmartContract;
using System;
using System.Linq;

namespace NeoSharp.Core.Helpers
{
    public class ContractHelper
    {
        /// <summary>
        /// Creates the single public key redeem contract ('regular account')
        /// </summary>
        /// <returns>The single public key redeem contract.</returns>
        /// <param name="scrypt">Scrypt.</param>
        /// <param name="publicKey">Public key.</param>
        public Contract CreateSinglePublicKeyRedeemContract(ICrypto scrypt, ECPoint publicKey){
            String contractHexCode;
            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitPush(publicKey.EncodedData);
                sb.Emit(EVMOpCode.CHECKSIG);
                contractHexCode = sb.ToArray().ToHexString();
            }

            ContractParameterType returnType = ContractParameterType.Void; 
            ContractParameterType[] parameters = { ContractParameterType.Signature };

            Code contractCode = new Code {
                //TODO: Double check if this is correct
                Script = contractHexCode,
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
        /// <param name="scrypt">Scrypt.</param>
        /// <param name="numberOfRequiredPublicKeys">Number of required public keys.</param>
        /// <param name="publicKeys">Public keys.</param>
        public Contract CreateMultiplePublicKeyRedeemContract(ICrypto scrypt, int numberOfRequiredPublicKeys, ECPoint[] publicKeys){
            string contractHexCode;

            if (!((1 <= numberOfRequiredPublicKeys) 
                  && (numberOfRequiredPublicKeys <= publicKeys.Length) 
                  && (publicKeys.Length <= 1024)))
                throw new ArgumentException("Invalid public keys. ");
            
            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitPush(numberOfRequiredPublicKeys);
                foreach (ECPoint publicKey in publicKeys.OrderBy(p => p))
                {
                    sb.EmitPush(publicKey.EncodedData);
                }
                sb.EmitPush(publicKeys.Length);
                sb.Emit(EVMOpCode.CHECKMULTISIG);
                contractHexCode = sb.ToArray().ToHexString();
            }

            ContractParameterType returnType = ContractParameterType.Void; 
            ContractParameterType[] parameters = Enumerable.Repeat(ContractParameterType.Signature, numberOfRequiredPublicKeys).ToArray();

            Code contractCode = new Code
            {
                //TODO: Double check if this is correct
                Script = contractHexCode,
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
    }
}
