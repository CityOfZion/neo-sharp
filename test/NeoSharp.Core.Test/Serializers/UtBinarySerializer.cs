using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Models;
using NeoSharp.Core.Models.OperationManger;
using NeoSharp.Core.Test.Types;
using NeoSharp.Cryptography;
using NeoSharp.TestHelpers;
using NeoSharp.Types;

namespace NeoSharp.Core.Test.Serializers
{
    [TestClass]
    public class UtBinarySerializer : TestBase
    {
        private IBinarySerializer _serializer;

        [TestInitialize]
        public void WarmUpSerializer()
        {
            BinarySerializer.RegisterTypes(typeof(SetTest));
            _serializer = new BinarySerializer(typeof(BlockHeader).Assembly, typeof(UtBinarySerializer).Assembly, typeof(Fixed8).Assembly);
        }

        public class SetTest
        {
            [BinaryProperty(0)]
            public HashSet<int> Set = new HashSet<int>();

            [BinaryProperty(1)]
            public Dictionary<int, string> Dictionary = new Dictionary<int, string>();
        }

        public class ReadOnlyDummyClass
        {
            [BinaryProperty(0)]
            public SetTest Test { get; private set; }

            public ReadOnlyDummyClass(SetTest test)
            {
                Test = test;
            }
        }

        public class ReadOnlyListDummyClass
        {
            [BinaryProperty(0)]
            public IReadOnlyList<int> List { get; private set; }

            public ReadOnlyListDummyClass(IReadOnlyList<int> list)
            {
                List = list;
            }
        }

        [TestMethod]
        public void ReadOnlyList()
        {
            var list = new ReadOnlyListDummyClass(new List<int>(new int[] {
                RandomInt(),
                RandomInt(),
                RandomInt() }));

            var ret = _serializer.Serialize(list);
            var clone = _serializer.Deserialize<ReadOnlyListDummyClass>(ret);

            CollectionAssert.AreEqual(list.List.ToArray(), clone.List.ToArray());
        }

        [TestMethod]
        public void ReadOnly()
        {
            var set = new SetTest();

            for (int x = 0, m = RandomInt(byte.MaxValue) * 2; x < m; x++)
            {
                set.Set.Add(RandomInt());
                set.Dictionary.Add(RandomInt(), RandomString(RandomInt(byte.MaxValue)));
            }

            var original = new ReadOnlyDummyClass(set);

            // Check that is the same

            var retInside = _serializer.Serialize(original.Test);
            var ret = _serializer.Serialize(original);

            CollectionAssert.AreEqual(ret, retInside);

            // Test deserialization

            var clone = _serializer.Deserialize<ReadOnlyDummyClass>(ret);

            Assert.AreEqual(set.Set.Count, clone.Test.Set.Count);
            Assert.AreEqual(set.Dictionary.Count, clone.Test.Dictionary.Count);

            CollectionAssert.AreEqual(set.Set.ToArray(), clone.Test.Set.ToArray());
            CollectionAssert.AreEqual(set.Dictionary.Keys.ToArray(), clone.Test.Dictionary.Keys.ToArray());
            CollectionAssert.AreEqual(set.Dictionary.Values.ToArray(), clone.Test.Dictionary.Values.ToArray());
        }

        [TestMethod]
        public void Serialize_Sets()
        {
            var set = new SetTest();

            for (int x = 0, m = RandomInt(byte.MaxValue) * 2; x < m; x++)
            {
                set.Set.Add(RandomInt());
                set.Dictionary.Add(RandomInt(), RandomString(RandomInt(ushort.MaxValue)));
            }

            var ret = _serializer.Serialize(set);
            var clone = _serializer.Deserialize<SetTest>(ret);

            Assert.AreEqual(set.Set.Count, clone.Set.Count);
            Assert.AreEqual(set.Dictionary.Count, clone.Dictionary.Count);

            CollectionAssert.AreEqual(set.Set.ToArray(), clone.Set.ToArray());
            CollectionAssert.AreEqual(set.Dictionary.Keys.ToArray(), clone.Dictionary.Keys.ToArray());
            CollectionAssert.AreEqual(set.Dictionary.Values.ToArray(), clone.Dictionary.Values.ToArray());
        }

        [TestMethod]
        public void DeserializeRecursive()
        {
            var data = new byte[]
            {
                0x01,0x00,
                0x01,
                0x02,
                0x03,0x00,
                0x04,0x00,
                0x05,0x00,0x00,0x00,
                0x06,0x00,0x00,0x00,
                0x07,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                0x08,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                0x05,0x01,0x02,0x03,0x04,0x05,
                0xcd,0xcc,0xcc,0xcc,0xcc,0xcc,0x25,0x40,
                0x00,
            };

            List<DummyParent> ls = new List<DummyParent>
            {
                _serializer.Deserialize<DummyParent>(data),
                (DummyParent)_serializer.Deserialize(data, typeof(DummyParent))
            };

            using (var ms = new MemoryStream(data))
            {
                ls.Add((DummyParent)_serializer.Deserialize(ms, typeof(DummyParent)));
                ms.Seek(0, SeekOrigin.Begin);
                ls.Add(_serializer.Deserialize<DummyParent>(ms));
            }

            using (var ms = new MemoryStream(data))
            using (var mr = new BinaryReader(ms))
            {
                ls.Add((DummyParent)_serializer.Deserialize(mr, typeof(DummyParent)));
                ms.Seek(0, SeekOrigin.Begin);
                ls.Add(_serializer.Deserialize<DummyParent>(mr));
            }

            foreach (var parent in ls)
            {
                (parent.A == 1).Should().BeTrue();

                var child = parent.B;

                (child.A == 1).Should().BeTrue();
                (child.B == 2).Should().BeTrue();
                (child.C == 3).Should().BeTrue();
                (child.D == 4).Should().BeTrue();
                (child.E == 5).Should().BeTrue();
                (child.F == 6).Should().BeTrue();
                (child.G == 7).Should().BeTrue();
                (child.H == 8).Should().BeTrue();
                child.I.SequenceEqual(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 }).Should().BeTrue();
                (child.J == 10.9).Should().BeTrue();
                child.K.Should().BeFalse();
            }
        }

        [TestMethod]
        public void Deserialize()
        {
            var actual = _serializer.Deserialize<Dummy>(new byte[]
            {
                0x01,
                0x02,
                0x03,0x00,
                0x04,0x00,
                0x05,0x00,0x00,0x00,
                0x06,0x00,0x00,0x00,
                0x07,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                0x08,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                0x05,0x01,0x02,0x03,0x04,0x05,
                0xcd,0xcc,0xcc,0xcc,0xcc,0xcc,0x25,0x40,
                0x01,
            });

            (actual.A == 1).Should().BeTrue();
            (actual.B == 2).Should().BeTrue();
            (actual.C == 3).Should().BeTrue();
            (actual.D == 4).Should().BeTrue();
            (actual.E == 5).Should().BeTrue();
            (actual.F == 6).Should().BeTrue();
            (actual.G == 7).Should().BeTrue();
            (actual.H == 8).Should().BeTrue();
            actual.I.SequenceEqual(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 }).Should().BeTrue();
            (actual.J == 10.9).Should().BeTrue();
            actual.K.Should().BeTrue();
        }

        [TestMethod]
        public void SerializeRecursive()
        {
            var parent = new DummyParent
            {
                A = 1,
                B = new Dummy
                {
                    A = 1,
                    B = 2,
                    C = 3,
                    D = 4,
                    E = 5,
                    F = 6,
                    G = 7,
                    H = 8,
                    I = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 },
                    J = 10.9,
                    K = true
                }
            };

            var ret = new byte[]
             {
                0x01,0x00,
                0x01,
                0x02,
                0x03,0x00,
                0x04,0x00,
                0x05,0x00,0x00,0x00,
                0x06,0x00,0x00,0x00,
                0x07,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                0x08,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                0x05, 0x01, 0x02, 0x03, 0x04, 0x05,
                0xcd,0xcc,0xcc,0xcc,0xcc,0xcc,0x25,0x40,
                0x01,
             };

            _serializer.Serialize(parent).SequenceEqual(ret).Should().BeTrue();

            using (var ms = new MemoryStream())
            {
                _serializer.Serialize(parent, ms);
                ms.ToArray().SequenceEqual(ret).Should().BeTrue();

                ms.SetLength(0);

                using (var mw = new BinaryWriter(ms))
                {
                    _serializer.Serialize(parent, mw);
                    ms.ToArray().SequenceEqual(ret).Should().BeTrue();
                }
            }
        }

        [TestMethod]
        public void Serialize()
        {
            var actual = new Dummy
            {
                A = 1,
                B = 2,
                C = 3,
                D = 4,
                E = 5,
                F = 6,
                G = 7,
                H = 8,
                I = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 },
                J = 10.9,
                K = false
            };

            _serializer.Serialize(actual).SequenceEqual(new byte[]
                {
                    0x01,
                    0x02,
                    0x03,0x00,
                    0x04,0x00,
                    0x05,0x00,0x00,0x00,
                    0x06,0x00,0x00,0x00,
                    0x07,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                    0x08,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                    0x05, 0x01, 0x02, 0x03, 0x04, 0x05,
                    0xcd,0xcc,0xcc,0xcc,0xcc,0xcc,0x25,0x40,
                    0x00
                }
            ).Should().BeTrue();
        }

        [TestMethod]
        public void Serialize_EnumArray()
        {
            var test = new CoinState[] { CoinState.Confirmed, CoinState.Locked };
            var copy = BinarySerializer.Default.Deserialize<CoinState[]>(BinarySerializer.Default.Serialize(test));

            CollectionAssert.AreEqual(test, copy);
        }

        [TestMethod]
        public void DeserializeReadOnly()
        {
            var readOnly = new DummyReadOnly();
            var copy = _serializer.Deserialize<DummyReadOnly>(_serializer.Serialize(readOnly));

            Assert.AreEqual(readOnly.A, copy.A);
        }

        [TestMethod]
        public void AddrPayloadSerialize()
        {
            var original = new AddrPayload
            {
                Address = new[]
                {
                    new NetworkAddressWithTime
                    {
                        EndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1").MapToIPv6(), ushort.MaxValue ),
                        Services = ulong.MaxValue,
                        Timestamp = uint.MaxValue,
                    },
                    new NetworkAddressWithTime
                    {
                        EndPoint = new IPEndPoint(IPAddress.Parse("::01").MapToIPv6(), ushort.MinValue ),
                        Services = ulong.MinValue,
                        Timestamp = uint.MinValue,
                    }
                }
            };

            var copy = _serializer.Deserialize<AddrPayload>(_serializer.Serialize(original));

            Assert.AreEqual(copy.Address.Length, original.Address.Length);

            for (int x = 0; x < copy.Address.Length; x++)
            {
                Assert.AreEqual(copy.Address[x].EndPoint.ToString(), original.Address[x].EndPoint.ToString());
                Assert.AreEqual(copy.Address[x].Timestamp, original.Address[x].Timestamp);
                Assert.AreEqual(copy.Address[x].Services, original.Address[x].Services);
            }
        }

        [TestMethod]
        public void SerializeDeserialize_Fixed8()
        {
            var original = new Fixed8(long.MaxValue);
            var copy = _serializer.Deserialize<Fixed8>(_serializer.Serialize(original));

            Assert.AreEqual(original, copy);
        }

        [TestMethod]
        public void SerializeDeserialize_NetworkAddressWithTime()
        {
            var original = new NetworkAddressWithTime
            {
                EndPoint = new IPEndPoint(IPAddress.Broadcast.MapToIPv6(), 0),
                Services = ulong.MaxValue,
                Timestamp = uint.MaxValue
            };

            var copy = _serializer.Deserialize<NetworkAddressWithTime>(_serializer.Serialize(original));

            Assert.AreEqual(original.Timestamp, copy.Timestamp);
            Assert.AreEqual(original.Services, copy.Services);
            Assert.AreEqual(original.EndPoint.ToString(), copy.EndPoint.ToString());
        }

        [TestMethod]
        public void SerializeDeserialize_UInt256()
        {
            var rand = new Random(Environment.TickCount);
            var hash = new byte[UInt256.BufferLength];
            rand.NextBytes(hash);

            var original = new UInt256(hash);
            var copy = _serializer.Deserialize<UInt256>(_serializer.Serialize(original));

            Assert.AreEqual(original, copy);
        }

        [TestMethod]
        public void SerializeDeserialize_UInt160()
        {
            var rand = new Random(Environment.TickCount);
            var hash = new byte[UInt160.BufferLength];
            rand.NextBytes(hash);

            var original = new UInt160(hash);
            var copy = _serializer.Deserialize<UInt160>(_serializer.Serialize(original));

            Assert.AreEqual(original, copy);
        }

        [TestMethod]
        public void BlockSerialize()
        {
            var witnessOperationsManager = new WitnessOperationsManager(Crypto.Default);
            var blockHeaderOperationsManager = new BlockHeaderOperationsManager(Crypto.Default, BinarySerializer.Default, witnessOperationsManager);

            var blockHeader = new Block
            {
                ConsensusData = 100_000_000,
                Hash = UInt256.Zero,
                Index = 0,
                MerkleRoot = UInt256.Zero,
                NextConsensus = UInt160.Zero,
                PreviousBlockHash = UInt256.Zero,
                Timestamp = 3,
                Version = 4,
                Witness = new Witness
                {
                    InvocationScript = new byte[0],
                    VerificationScript = new byte[0],
                },
                Transactions = new Transaction[] { new InvocationTransaction()
                    {
                    Attributes=new TransactionAttribute[]{ },
                    Inputs=new CoinReference[]{ },
                    Outputs=new TransactionOutput[]{},
                    Witness=new Witness[]{ },
                    Script=new byte[]{ 0x01 },
                    Version=0
                    }
                }
            };

            blockHeaderOperationsManager.Sign(blockHeader);

            var blockHeaderCopy = _serializer.Deserialize<Block>(_serializer.Serialize(blockHeader));
            blockHeaderOperationsManager.Sign(blockHeaderCopy);

            Assert.AreEqual(blockHeader.ConsensusData, blockHeaderCopy.ConsensusData);
            Assert.AreEqual(blockHeader.Hash, blockHeaderCopy.Hash);
            Assert.AreEqual(blockHeader.Index, blockHeaderCopy.Index);
            Assert.AreEqual(blockHeader.MerkleRoot, blockHeaderCopy.MerkleRoot);
            Assert.AreEqual(blockHeader.NextConsensus, blockHeaderCopy.NextConsensus);
            Assert.AreEqual(blockHeader.PreviousBlockHash, blockHeaderCopy.PreviousBlockHash);
            Assert.AreEqual(blockHeader.Timestamp, blockHeaderCopy.Timestamp);
            Assert.AreEqual(blockHeader.Version, blockHeaderCopy.Version);

            Assert.IsTrue(blockHeader.Witness.InvocationScript.SequenceEqual(blockHeaderCopy.Witness.InvocationScript));
            Assert.IsTrue(blockHeader.Witness.VerificationScript.SequenceEqual(blockHeaderCopy.Witness.VerificationScript));
        }
    }
}