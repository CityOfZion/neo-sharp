using System;
using System.Linq;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Models;
using NeoSharp.Core.Types;
using NeoSharp.VM;

namespace NeoSharp.Core.Blockchain
{
    public class Genesis
    {
#pragma warning disable CS0612 // Type or member is obsolete

        /// <summary>
        /// Static constructor
        /// </summary>
        static Genesis()
        {
            BinarySerializer.RegisterTypes(typeof(PublishTransaction).Assembly, typeof(BlockHeader).Assembly);

            #region Tokens

            GoverningToken = new RegisterTransaction
            {
                AssetType = AssetType.GoverningToken,
                Name = "[{\"lang\":\"zh-CN\",\"name\":\"小蚁股\"},{\"lang\":\"en\",\"name\":\"AntShare\"}]",
                Amount = Fixed8.FromDecimal(100000000),
                Precision = 0,
                Owner = ECPoint.Infinity,
                Admin = (new[] { (byte)EVMOpCode.PUSH1 }).ToScriptHash(),
                Attributes = new TransactionAttribute[0],
                Inputs = new CoinReference[0],
                Outputs = new TransactionOutput[0],
                Scripts = new Witness[0]
            };

            UtilityToken = new RegisterTransaction
            {
                AssetType = AssetType.UtilityToken,
                Name = "[{\"lang\":\"zh-CN\",\"name\":\"小蚁币\"},{\"lang\":\"en\",\"name\":\"AntCoin\"}]",
                Amount = Fixed8.FromDecimal(GenerationAmount.Sum(p => p) * DecrementInterval),
                Precision = 8,
                Owner = ECPoint.Infinity,
                Admin = (new[] { (byte)EVMOpCode.PUSH0 }).ToScriptHash(),
                Attributes = new TransactionAttribute[0],
                Inputs = new CoinReference[0],
                Outputs = new TransactionOutput[0],
                Scripts = new Witness[0]
            };

            // Compute hash

            UtilityToken.UpdateHash(BinarySerializer.Default, ICrypto.Default);
            GoverningToken.UpdateHash(BinarySerializer.Default, ICrypto.Default);

            #endregion

            #region Block

            GenesisBlock = new Block
            {
                PreviousBlockHash = UInt256.Zero,
                Timestamp = (new DateTime(2016, 7, 15, 15, 8, 21, DateTimeKind.Utc)).ToTimestamp(),
                Index = 0,
                ConsensusData = 2083236893, //向比特币致敬
                NextConsensus = GetConsensusAddress(StandbyValidators),
                Script = new Witness
                {
                    InvocationScript = new byte[0],
                    VerificationScript = new[] { (byte)EVMOpCode.PUSH1 }
                },
                Transactions = new Transaction[]
                   {
                        new MinerTransaction
                        {
                            Nonce = 2083236893,
                            Attributes = new TransactionAttribute[0],
                            Inputs = new CoinReference[0],
                            Outputs = new TransactionOutput[0],
                            Scripts = new Witness[0]
                        },

                        GoverningToken,
                        UtilityToken,

                        new IssueTransaction
                        {
                            Attributes = new TransactionAttribute[0],
                            Inputs = new CoinReference[0],
                            Outputs = new[]
                            {
                                new TransactionOutput
                                {
                                    AssetId = GoverningToken.Hash,
                                    Value = GoverningToken.Amount,
                                    ScriptHash = CreateMultiSigRedeemScript(StandbyValidators.Length / 2 + 1, StandbyValidators).ToScriptHash()
                                }
                            },
                            Scripts = new[]
                            {
                                new Witness
                                {
                                    InvocationScript = new byte[0],
                                    VerificationScript = new[] { (byte)EVMOpCode.PUSH1 }
                                }
                            }
                        }
                   }
            };

            // Compute hash

            GenesisBlock.UpdateHash(BinarySerializer.Default, ICrypto.Default);

            #endregion
        }

        public const uint DecrementInterval = 2000000;
        public static readonly uint[] GenerationAmount = { 8, 7, 6, 5, 4, 3, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

        /// <summary>
        /// Stand by validators
        /// </summary>
        public static readonly ECPoint[] StandbyValidators =

             // TODO: Extract from config

             new string[]
                {
                "03b209fd4f53a7170ea4444e0cb0a6bb6a53c2bd016926989cf85f9b0fba17a70c",
                "02df48f60e8f3e01c48ff40b9b7f1310d7a8b2a193188befe1c2e3df740e895093",
                "03b8d9d5771d8f513aa0869b9cc8d50986403b78c6da36890638c3d46a5adce04a",
                "02ca0e27697b9c248f6f16e085fd0061e26f44da85b58ee835c110caa5ec3ba554",
                "024c7b7fb6c310fccf1ba33b082519d82964ea93868d676662d4a59ad548df0e7d",
                "02aaec38470f6aad0042c6e877cfd8087d2676b0f516fddd362801b9bd3936399e",
                "02486fd15702c4490a26703112a5cc1d0923fd697a33406bd5a1c00e0013b09a70"
                }
                .Select(u => new ECPoint(u.HexToBytes())).ToArray();

        /// <summary>
        /// NEO
        /// </summary>
        public static readonly RegisterTransaction GoverningToken;
        /// <summary>
        /// Gas
        /// </summary>
        public static readonly RegisterTransaction UtilityToken;
#pragma warning restore CS0612 // Type or member is obsolete

        /// <summary>
        /// Genesis
        /// </summary>
        public static readonly Block GenesisBlock;

        /// <summary>
        /// Get consensus address
        /// </summary>
        /// <param name="validators">Validators</param>
        /// <returns>Return byte address</returns>
        public static UInt160 GetConsensusAddress(params ECPoint[] validators)
        {
            return CreateMultiSigRedeemScript(validators.Length - (validators.Length - 1) / 3, validators).ToScriptHash();
        }

        /// <summary>
        /// CreateMultiSigRedeemScript
        /// </summary>
        /// <param name="m">Min</param>
        /// <param name="publicKeys">Public key</param>
        /// <returns>Return byte array</returns>
        public static byte[] CreateMultiSigRedeemScript(int m, params ECPoint[] publicKeys)
        {
            if (!(1 <= m && m <= publicKeys.Length && publicKeys.Length <= 1024))
            {
                throw new ArgumentException(nameof(publicKeys));
            }

            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitPush(m);

                foreach (ECPoint publicKey in publicKeys.OrderBy(p => p))
                {
                    sb.EmitPush(publicKey.EncodedData);
                }

                sb.EmitPush(publicKeys.Length);
                sb.Emit(EVMOpCode.CHECKMULTISIG);

                return sb.ToArray();
            }
        }
    }
}