using System;
using System.Collections.Generic;
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
            var script = CreateSinglePublicKeyRedeemScript(publicKey);
            var contractCode = new Code {
                Script = script.HexToBytes(),
                ScriptHash = script.HexToBytes().ToScriptHash(),
                ReturnType = ContractParameterType.Void,
                Parameters = new[] { ContractParameterType.Signature }
            };

            return new Contract
            {
                Code = contractCode
            };
        }

        public static string CreateSinglePublicKeyRedeemScript(ECPoint publicKey)
        {
            if (publicKey == null) throw new ArgumentNullException(nameof(publicKey));

            using (var sb = new ScriptBuilder())
            {
                sb.EmitPush(publicKey.EncodedData);
                sb.Emit(EVMOpCode.CHECKSIG);

                return sb.ToArray().ToHexString();
            }
        }

        /// <summary>
        /// Creates the multiple public key redeem contract ('multisig account')
        /// </summary>
        /// <returns>The multiple public key redeem contract.</returns>
        /// <param name="numberOfRequiredPublicKeys">Number of required public keys.</param>
        /// <param name="publicKeys">Public keys.</param>
        public static Contract CreateMultiplePublicKeyRedeemContract(int numberOfRequiredPublicKeys, ECPoint[] publicKeys)
        {
            var script = CreateMultiplePublicKeyRedeemScript(numberOfRequiredPublicKeys, publicKeys);
            var code = new Code
            {
                Script = script,
                ScriptHash = script.ToScriptHash(),
                ReturnType = ContractParameterType.Void,
                Parameters = Enumerable.Repeat(ContractParameterType.Signature, numberOfRequiredPublicKeys).ToArray()
            };

            return new Contract
            {
                Code = code
            };
        }

        public static byte[] CreateMultiplePublicKeyRedeemScript(int numberOfRequiredPublicKeys, IReadOnlyCollection<ECPoint> publicKeys)
        {
            if (publicKeys == null) throw new ArgumentNullException(nameof(publicKeys));
            if (!(1 <= numberOfRequiredPublicKeys
                  && numberOfRequiredPublicKeys <= publicKeys.Count
                  && publicKeys.Count <= 1024))
                throw new ArgumentException("Invalid public keys");

            using (var sb = new ScriptBuilder())
            {
                sb.EmitPush(numberOfRequiredPublicKeys);

                foreach (var publicKey in publicKeys.OrderBy(p => p))
                {
                    sb.EmitPush(publicKey.EncodedData);
                }

                sb.EmitPush(publicKeys.Count);
                sb.Emit(EVMOpCode.CHECKMULTISIG);

                return sb.ToArray();
            }

        }
    }
}