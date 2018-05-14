using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Models;
using NeoSharp.Core.Test.Types;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Test.Serializers
{
    [TestClass]
    public class UtBinarySerializer
    {
        private IBinarySerializer _serializer;

        [TestInitialize]
        public void WarmSerializer()
        {
            _serializer = new BinarySerializer(typeof(Block).Assembly, typeof(UtBinarySerializer).Assembly);
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
            var parent = new DummyParent()
            {
                A = 1,
                B = new Dummy()
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
            var actual = new Dummy()
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
        public void ReadOnly()
        {
            var readOnly = new DummyReadOnly();
            var copy = _serializer.Deserialize<DummyReadOnly>(_serializer.Serialize(readOnly));

            Assert.AreEqual(readOnly.A, copy.A);
        }

        [TestMethod]
        public void BlockSerialize()
        {
            var block = new Block()
            {
                Confirmations = 1,
                ConsensusData = 100_000_000,
                Hash = "Hash",
                Index = 0,
                MerkleRoot = UInt256.Zero,
                NextBlockHash = UInt256.Zero,
                NextConsensus = UInt160.Zero,
                PreviousBlockHash = UInt256.Zero,
                Size = 2,
                Timestamp = 3,
                Version = 4,
                Script = new Witness
                {
                    InvocationScript = "InvocationScript",
                    VerificationScript = new byte[0],
                },
                TxHashes = new[] { "a", "b", "c" }
            };

            var blockCopy = _serializer.Deserialize<Block>(_serializer.Serialize(block));

            Assert.AreEqual(block.Confirmations, blockCopy.Confirmations);
            Assert.AreEqual(block.ConsensusData, blockCopy.ConsensusData);
            Assert.AreEqual(block.Hash, blockCopy.Hash);
            Assert.AreEqual(block.Index, blockCopy.Index);
            Assert.AreEqual(block.MerkleRoot, blockCopy.MerkleRoot);
            Assert.AreEqual(block.NextConsensus, blockCopy.NextConsensus);
            Assert.AreEqual(block.PreviousBlockHash, blockCopy.PreviousBlockHash);
            Assert.AreEqual(block.Size, blockCopy.Size);
            Assert.AreEqual(block.Timestamp, blockCopy.Timestamp);
            Assert.AreEqual(block.Version, blockCopy.Version);

            Assert.AreEqual(block.Script.InvocationScript, blockCopy.Script.InvocationScript);
            Assert.IsTrue(block.Script.VerificationScript.SequenceEqual(blockCopy.Script.VerificationScript));

            Assert.IsTrue(block.TxHashes.SequenceEqual(blockCopy.TxHashes));
        }
    }
}