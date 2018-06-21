using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Models;
using NeoSharp.Core.SmartContract;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Test.Serializers
{
    [TestClass]
    public class UtBinarySerializerTx
    {
        private Random _random;
        private ICrypto _crypto;
        private IBinarySerializer _serializer;
        private IBinaryDeserializer _deserializer;

        [TestInitialize]
        public void WarmUpSerializer()
        {
            _random = new Random(Environment.TickCount);
            _crypto = new BouncyCastleCrypto();
            _serializer = new BinarySerializer(typeof(BlockHeader).Assembly, typeof(UtBinarySerializer).Assembly);
            _deserializer = new BinaryDeserializer(typeof(BlockHeader).Assembly, typeof(UtBinarySerializer).Assembly);
        }

        [TestMethod]
        public void SerializeDeserialize_TransactionOutput()
        {
            var original = RandomTransactionOutputs(1).FirstOrDefault();

            var ret = _serializer.Serialize(original);
            var copy = _deserializer.Deserialize<TransactionOutput>(ret);

            Assert.AreEqual(original.AssetId, copy.AssetId);
            Assert.AreEqual(original.ScriptHash, copy.ScriptHash);
            Assert.AreEqual(original.Value, copy.Value);
        }

        [TestMethod]
        public void SerializeDeserialize_InvocationTransaction()
        {
            var original = new InvocationTransaction()
            {
                Version = 0x01,
                Gas = RandomFixed8(true, 1).FirstOrDefault(),
                Script = RandomBytes(_random.Next(1, short.MaxValue)),
            };

            FillRandomTx(original);

            var ret = _serializer.Serialize(original);
            var copy = _deserializer.Deserialize<Transaction>(ret);
            var copy2 = _deserializer.Deserialize<InvocationTransaction>(ret);

            // Check exclusive data

            Assert.AreEqual(original.Gas, ((InvocationTransaction)copy).Gas);
            CollectionAssert.AreEqual(original.Script, ((InvocationTransaction)copy).Script);

            Assert.AreEqual(original.Gas, ((InvocationTransaction)copy).Gas);
            CollectionAssert.AreEqual(original.Script, ((InvocationTransaction)copy2).Script);

            // Check base data

            EqualTx(original, copy, copy2);
        }

        [TestMethod]
        public void SerializeDeserialize_MinerTransaction()
        {
            var original = new MinerTransaction()
            {
                Version = 0x00,
                Nonce = (uint)_random.Next(0, int.MaxValue)
            };

            FillRandomTx(original);

            var ret = _serializer.Serialize(original);
            var copy = _deserializer.Deserialize<Transaction>(ret);
            var copy2 = _deserializer.Deserialize<MinerTransaction>(ret);

            // Check exclusive data

            Assert.AreEqual(original.Nonce, ((MinerTransaction)copy).Nonce);
            Assert.AreEqual(original.Nonce, ((MinerTransaction)copy2).Nonce);

            // Check base data

            EqualTx(original, copy, copy2);
        }

        [TestMethod]
        public void SerializeDeserialize_ContractTransaction()
        {
            var original = new ContractTransaction()
            {
                Version = 0x00,
            };

            FillRandomTx(original);

            var ret = _serializer.Serialize(original);
            var copy = _deserializer.Deserialize<Transaction>(ret);
            var copy2 = _deserializer.Deserialize<ContractTransaction>(ret);

            // Check base data

            EqualTx(original, copy, copy2);
        }

        [TestMethod]
        public void SerializeDeserialize_StateTransaction()
        {
            var original = new StateTransaction()
            {
                Version = 0x01,
                Descriptors = RandomStateDescriptors(_random.Next(1, 16)).ToArray()
            };

            FillRandomTx(original);

            var ret = _serializer.Serialize(original);
            var copy = _deserializer.Deserialize<Transaction>(ret);
            var copy2 = _deserializer.Deserialize<StateTransaction>(ret);

            // Check exclusive data

            foreach (var check in new StateTransaction[] { (StateTransaction)copy, copy2 })
            {
                //Assert.AreEqual(original.ReturnType, check.ReturnType);
                //CollectionAssert.AreEqual(original.ParameterList, check.ParameterList);
            }

            // Check base data

            EqualTx(original, copy, copy2);
        }

        [TestMethod]
        public void SerializeDeserialize_PublishTransaction()
        {
            var original = new PublishTransaction()
            {
                Version = 0x01,
                Author = RandomString(1, 255),
                Email = RandomString(1, 255),
                Description = RandomString(1, 255),
                Name = RandomString(1, 255),
                CodeVersion = RandomString(1, 255),
                NeedStorage = _random.Next() % 2 == 0,
                Script = RandomBytes(_random.Next(1, ushort.MaxValue)),
                ParameterList = RandomParameterList(_random.Next(1, 10)).ToArray(),
                ReturnType = RandomParameterList(1).FirstOrDefault(),
            };

            FillRandomTx(original);

            var ret = _serializer.Serialize(original);
            var copy = _deserializer.Deserialize<Transaction>(ret);
            var copy2 = _deserializer.Deserialize<PublishTransaction>(ret);

            // Check exclusive data

            foreach (var check in new PublishTransaction[] { (PublishTransaction)copy, copy2 })
            {
                Assert.AreEqual(original.Author, check.Author);
                Assert.AreEqual(original.Email, check.Email);
                Assert.AreEqual(original.Description, check.Description);
                Assert.AreEqual(original.Name, check.Name);
                Assert.AreEqual(original.CodeVersion, check.CodeVersion);
                Assert.AreEqual(original.NeedStorage, check.NeedStorage);
                Assert.AreEqual(original.Author, check.Author);
                Assert.AreEqual(original.ReturnType, check.ReturnType);
                CollectionAssert.AreEqual(original.Script, check.Script);
                CollectionAssert.AreEqual(original.ParameterList, check.ParameterList);
            }

            // Check base data

            EqualTx(original, copy, copy2);
        }

        [TestMethod]
        public void SerializeDeserialize_IssueTransaction()
        {
            var original = new IssueTransaction()
            {
                Version = 0x00,
            };

            FillRandomTx(original);

            var ret = _serializer.Serialize(original);
            var copy = _deserializer.Deserialize<Transaction>(ret);
            var copy2 = _deserializer.Deserialize<IssueTransaction>(ret);

            // Check base data

            EqualTx(original, copy, copy2);
        }

        [TestMethod]
        public void SerializeDeserialize_ClaimTransaction()
        {
            var original = new ClaimTransaction()
            {
                Claims = RandomCoinReferences(_random.Next(1, 255)).ToArray(),
                Version = 0x00,
            };

            FillRandomTx(original);

            var ret = _serializer.Serialize(original);
            var copy = _deserializer.Deserialize<Transaction>(ret);
            var copy2 = _deserializer.Deserialize<ClaimTransaction>(ret);

            // Check exclusive data

            CollectionAssert.AreEqual(original.Claims, ((ClaimTransaction)copy).Claims);
            CollectionAssert.AreEqual(original.Claims, copy2.Claims);

            // Check base data

            EqualTx(original, copy, copy2);
        }

        private void FillRandomTx(Transaction tx)
        {
            tx.Attributes = RandomTransactionAtrributes().ToArray();
            tx.Inputs = RandomCoinReferences(_random.Next(1, 255)).ToArray();
            tx.Outputs = RandomTransactionOutputs().ToArray();
            tx.Scripts = RandomWitness().ToArray();

            tx.UpdateHash(_serializer, _crypto);
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

                copy.UpdateHash(_serializer, _crypto);

                Assert.AreEqual(original.Hash, copy.Hash);
            }
        }

        [TestMethod]
        public void SerializeDeserialize_TransactionAttribute()
        {
            // Checks

            foreach (var ori in RandomTransactionAtrributes(255))
            {
                var ret = _serializer.Serialize(ori);
                var copy = _deserializer.Deserialize<TransactionAttribute>(ret);

                Assert.AreEqual(ori.Usage, copy.Usage);
                Assert.IsTrue(ori.Data.SequenceEqual(copy.Data));
            }
        }

        // Random generators

        private IEnumerable<Witness> RandomWitness(int count = 1)
        {
            for (; count >= 0; count--)
            {
                var invocation = new byte[_random.Next(1, 255)];
                var verification = new byte[_random.Next(1, 255)];

                _random.NextBytes(invocation);
                _random.NextBytes(verification);

                yield return new Witness()
                {
                    InvocationScript = invocation,
                    VerificationScript = verification
                };
            }
        }

        private IEnumerable<TransactionAttribute> RandomTransactionAtrributes(int count = 1)
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

                _random.NextBytes(ori.Data);

                switch (ori.Usage)
                {
                    case TransactionAttributeUsage.DescriptionUrl:
                    case TransactionAttributeUsage.Description: ori.Data[0] = (byte)(ori.Data.Length - 1); break;
                    case TransactionAttributeUsage.ECDH02:
                    case TransactionAttributeUsage.ECDH03: ori.Data[0] = (byte)ori.Usage; break;
                }

                if (count <= 0) yield break;

                yield return ori;
                count--;
            }
        }

        private string RandomString(int min, int max)
        {
            return RandomString(_random.Next(min, max));
        }

        private string RandomString(int length)
        {
            char[] alphabet = "qwertyuioplkjhgfdsazxcvbnm0987654321_".ToArray();

            var value = new char[length];

            for (var x = 0; x < value.Length; x++)
            {
                value[x] = alphabet[_random.Next() % alphabet.Length];
            }

            return new string(value, 0, value.Length);
        }

        private byte[] RandomBytes(int length)
        {
            var value = new byte[length];
            _random.NextBytes(value);

            return value;
        }

        private byte[] RandomBytes(int min, int max)
        {
            return RandomBytes(_random.Next(min, max));
        }

        private IEnumerable<StateDescriptor> RandomStateDescriptors(int count = 1)
        {
            for (; count >= 0; count--)
            {
                var ret = new StateDescriptor()
                {
                    Type = RandomEnum<StateType>(),
                    Value = RandomBytes(_random.Next(1, byte.MaxValue))
                };

                switch (ret.Type)
                {
                    case StateType.Account:
                        {
                            ret.Key = RandomBytes(20);
                            ret.Field = "Votes";
                            break;
                        }
                    case StateType.Validator:
                        {
                            ret.Key = RandomBytes(33);
                            ret.Field = "Registered";
                            break;
                        }
                }

                yield return ret;
            }
        }

        private IEnumerable<Fixed8> RandomFixed8(bool onlyPositive, int count = 1)
        {
            for (; count >= 0; count--)
            {
                var value = RandomBytes(8, 8);
                var lvalue = BitConverter.ToInt64(value, 0);

                if (onlyPositive && lvalue < 0)
                {
                    lvalue *= -1;
                }

                yield return new Fixed8(lvalue);
            }
        }

        private IEnumerable<TransactionOutput> RandomTransactionOutputs(int count = 1)
        {
            for (; count >= 0; count--)
            {
                yield return new TransactionOutput()
                {
                    AssetId = RandomUInt256(1).FirstOrDefault(),
                    ScriptHash = RandomUInt60(1).FirstOrDefault(),
                    Value = RandomFixed8(false, 1).FirstOrDefault(),
                };
            }
        }

        private IEnumerable<ContractParameterType> RandomParameterList(int count = 1)
        {
            for (; count >= 0; count--)
            {
                yield return RandomEnum<ContractParameterType>();
            }
        }

        private T RandomEnum<T>()
        {
            var values = Enum.GetValues(typeof(T));

            return (T)values.GetValue(_random.Next(0, values.Length));
        }

        private IEnumerable<UInt160> RandomUInt60(int count = 1)
        {
            for (; count >= 0; count--)
            {
                var data = RandomBytes(UInt160.BufferLength);

                yield return new UInt160(data);
            }
        }

        private IEnumerable<UInt256> RandomUInt256(int count = 1)
        {
            for (; count >= 0; count--)
            {
                var data = RandomBytes(UInt256.BufferLength);

                yield return new UInt256(data);
            }
        }

        private IEnumerable<CoinReference> RandomCoinReferences(int count = 1)
        {
            for (; count >= 0; count--)
                yield return new CoinReference()
                {
                    PrevHash = RandomUInt256(1).FirstOrDefault(),
                    PrevIndex = (ushort)_random.Next(ushort.MinValue, ushort.MaxValue),
                };
        }

    }
}