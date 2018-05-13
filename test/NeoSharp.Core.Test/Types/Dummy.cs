using NeoSharp.BinarySerialization;

namespace NeoSharp.Core.Test.Types
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

        [BinaryProperty(10)]
        public double J { get; set; }

        [BinaryProperty(11)]
        public bool K { get; set; }
    }
}