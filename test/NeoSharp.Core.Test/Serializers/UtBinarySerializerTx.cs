using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Models;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Test.Serializers
{
    [TestClass]
    public class UtBinarySerializerTx
    {
        private IBinarySerializer _serializer;
        private IBinaryDeserializer _deserializer;

        [TestInitialize]
        public void WarmUpSerializer()
        {
            _serializer = new BinarySerializer(typeof(BlockHeader).Assembly, typeof(UtBinarySerializer).Assembly);
            _deserializer = new BinaryDeserializer(typeof(BlockHeader).Assembly, typeof(UtBinarySerializer).Assembly);
        }

        [TestMethod]
        public void SerializeDeserialize_TransactionOutput()
        {
            var rand = new Random(Environment.TickCount);

            var bufAsset = new byte[UInt256.BufferLength];
            rand.NextBytes(bufAsset);
            var bufScript = new byte[UInt160.BufferLength];
            rand.NextBytes(bufScript);

            var original = new TransactionOutput()
            {
                AssetId = new UInt256(bufAsset),
                ScriptHash = new UInt160(bufScript),
                Value = new Fixed8(BitConverter.ToInt64(bufAsset.Take(8).ToArray(), 0))
            };

            var ret = _serializer.Serialize(original);
            var copy = _deserializer.Deserialize<TransactionOutput>(ret);

            Assert.AreEqual(original.AssetId, copy.AssetId);
            Assert.AreEqual(original.ScriptHash, copy.ScriptHash);
            Assert.AreEqual(original.Value, copy.Value);
        }

        [TestMethod]
        public void SerializeDeserialize_Tx_ClaimTransaction()
        {
            var rand = new Random(Environment.TickCount);

            var randomHash = new byte[UInt256.BufferLength];
            rand.NextBytes(randomHash);

            var original = new ClaimTransaction()
            {
                Claims = new CoinReference[]
                {
                    new CoinReference()
                    {
                        PrevIndex=(ushort)rand.Next(ushort.MinValue,ushort.MaxValue),
                        PrevHash=new UInt256(randomHash),
                    }
                }
            };

            FillRandomSharedTxData(rand, original);

            var ret = _serializer.Serialize(original);
            var copy = _deserializer.Deserialize<Transaction>(ret);
            var copy2 = _deserializer.Deserialize<ClaimTransaction>(ret);

            // Check exclusive data

            CollectionAssert.AreEqual(original.Claims, ((ClaimTransaction)copy).Claims);
            CollectionAssert.AreEqual(original.Claims, copy2.Claims);

            // Check base data

            EqualTx(original, copy, copy2);
        }

        private void FillRandomSharedTxData(Random rand, Transaction tx)
        {
            var randomHash = new byte[UInt256.BufferLength];
            rand.NextBytes(randomHash);

            tx.Hash = new UInt256(randomHash);

            tx.Attributes = GetAllTransactionAttributes(rand);
            tx.Inputs = new CoinReference[]
            {
                new CoinReference()
                    {
                        PrevIndex=(ushort)rand.Next(ushort.MinValue,ushort.MaxValue),
                        PrevHash=new UInt256(randomHash),
                    }
            };
            tx.Outputs = new TransactionOutput[]
            {
                new TransactionOutput()
                {
                     AssetId=new UInt256(randomHash),
                     Value=new Fixed8(BitConverter.ToInt64(randomHash,0)),
                     ScriptHash=new UInt160(randomHash.Take(UInt160.BufferLength).ToArray())
                }
            };
            tx.Scripts = new Witness[]
            {
                new Witness()
                {
                     InvocationScript = randomHash.Reverse().ToArray(),
                     VerificationScript = randomHash.Reverse().ToArray()
                }
            };
            tx.Version = 0;
        }

        void EqualTx(Transaction original, params Transaction[] copies)
        {
            foreach (var copy in copies)
            {
                Assert.AreEqual(original.GetType(), copy.GetType());

                Assert.AreEqual(original.Type, copy.Type);
                Assert.AreEqual(original.Version, copy.Version);

                CollectionAssert.AreEqual(original.Attributes, copy.Attributes);
                CollectionAssert.AreEqual(original.Inputs, copy.Inputs);
                CollectionAssert.AreEqual(original.Outputs, copy.Outputs);
                CollectionAssert.AreEqual(original.Scripts, copy.Scripts);

                // Recompute hash

                original.Hash = null;
                copy.Hash = null;

                Assert.AreEqual(original.Hash, copy.Hash);
            }
        }

        TransactionAttribute[] GetAllTransactionAttributes(Random rand)
        {
            var ret = new TransactionAttribute[]
            {
                new TransactionAttribute()
                {
                    Usage = TransactionAttributeUsage.ContractHash,
                    Data = new byte[32]
                },
                new TransactionAttribute()
                {
                    Usage = TransactionAttributeUsage.Vote,
                    Data = new byte[32]
                },
                new TransactionAttribute()
                {
                    Usage = TransactionAttributeUsage.Hash1,
                    Data = new byte[32]
                },
                new TransactionAttribute()
                {
                    Usage = TransactionAttributeUsage.Hash15,
                    Data = new byte[32]
                },
                new TransactionAttribute()
                {
                    Usage = TransactionAttributeUsage.ECDH02,
                    Data = new byte[33]
                },
                new TransactionAttribute()
                {
                    Usage = TransactionAttributeUsage.ECDH03,
                    Data = new byte[33]
                },
                new TransactionAttribute()
                {
                    Usage = TransactionAttributeUsage.Script,
                    Data = new byte[20]
                },
                new TransactionAttribute()
                {
                    Usage = TransactionAttributeUsage.DescriptionUrl,
                    Data = new byte[11]
                },
                new TransactionAttribute()
                {
                    Usage = TransactionAttributeUsage.Description,
                    Data = new byte[11]
                }
            };

            foreach (var ori in ret)
            {
                // Fill random data

                rand.NextBytes(ori.Data);

                switch (ori.Usage)
                {
                    case TransactionAttributeUsage.DescriptionUrl:
                    case TransactionAttributeUsage.Description: ori.Data[0] = (byte)(ori.Data.Length - 1); break;
                    case TransactionAttributeUsage.ECDH02:
                    case TransactionAttributeUsage.ECDH03: ori.Data[0] = (byte)ori.Usage; break;
                }
            }

            return ret;
        }

        [TestMethod]
        public void SerializeDeserialize_TransactionAttribute()
        {
            var rand = new Random(Environment.TickCount);

            var original = GetAllTransactionAttributes(rand);

            // Checks

            foreach (var ori in original)
            {
                var ret = _serializer.Serialize(ori);
                var copy = _deserializer.Deserialize<TransactionAttribute>(ret);

                Assert.AreEqual(ori.Usage, copy.Usage);
                Assert.IsTrue(ori.Data.SequenceEqual(copy.Data));
            }
        }
    }
}