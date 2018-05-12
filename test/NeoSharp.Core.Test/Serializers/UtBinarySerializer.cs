using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Models;
using NeoSharp.Core.Test.Types;
using System.Linq;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Test.Serializers
{
    [TestClass]
    public class UtBinarySerializer
    {
        [TestInitialize]
        public void WarmSerializer()
        {
            BinarySerializer.CacheTypesOf(typeof(Block).Assembly);
            BinarySerializer.CacheTypesOf(typeof(UtBinarySerializer).Assembly);
        }

        [TestMethod]
        public void DeserializeRecursive()
        {
            var parent = BinarySerializer.Deserialize<DummyParent>(new byte[]
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
            });

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
            (child.I.SequenceEqual(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 })).Should().BeTrue();
        }

        [TestMethod]
        public void Deserialize()
        {
            var actual = BinarySerializer.Deserialize<Dummy>(new byte[]
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
            });

            (actual.A == 1).Should().BeTrue();
            (actual.B == 2).Should().BeTrue();
            (actual.C == 3).Should().BeTrue();
            (actual.D == 4).Should().BeTrue();
            (actual.E == 5).Should().BeTrue();
            (actual.F == 6).Should().BeTrue();
            (actual.G == 7).Should().BeTrue();
            (actual.H == 8).Should().BeTrue();
            (actual.I.SequenceEqual(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 })).Should().BeTrue();
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
                    I = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 }
                }
            };

            (BinarySerializer.Serialize(parent).SequenceEqual(new byte[]
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
                0x05, 0x01, 0x02, 0x03, 0x04, 0x05
            }
            )).Should().BeTrue();
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
                I = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 }
            };

            (BinarySerializer.Serialize(actual).SequenceEqual(new byte[]
            {
                0x01,
                0x02,
                0x03,0x00,
                0x04,0x00,
                0x05,0x00,0x00,0x00,
                0x06,0x00,0x00,0x00,
                0x07,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                0x08,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                0x05, 0x01, 0x02, 0x03, 0x04, 0x05
            }
            )).Should().BeTrue();
        }

        [TestMethod]
        public void ReadOnly()
        {
            DummyReadOnly readOnly = new DummyReadOnly();
            var copy = BinarySerializer.Deserialize<DummyReadOnly>((BinarySerializer.Serialize(readOnly)));

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

            var blockCopy = BinarySerializer.Deserialize<Block>(BinarySerializer.Serialize(block));

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