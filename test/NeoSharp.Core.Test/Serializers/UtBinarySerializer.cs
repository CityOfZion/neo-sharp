using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Core.Serializers;
using System.Linq;

namespace NeoSharp.Core.Test.Serializers
{
    [TestClass]
    public class UtBinarySerializer
    {
        class Dummy
        {
            public byte Trash { get; set; }

            [BinaryProperty(1)]
            public byte A { get; set; }
            [BinaryProperty(2)]
            public sbyte B { get; set; }

            [BinaryProperty(3)]
            public short C { get; set; }
            [BinaryProperty(4)]
            public ushort D { get; set; }

            [BinaryProperty(5)]
            public int E { get; set; }
            [BinaryProperty(6)]
            public uint F { get; set; }

            [BinaryProperty(7)]
            public long G { get; set; }
            [BinaryProperty(8)]
            public ulong H { get; set; }

            [BinaryProperty(9)]
            public byte[] I { get; set; }
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
    }
}