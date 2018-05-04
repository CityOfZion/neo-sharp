using NeoSharp.BinarySerialization;

namespace NeoSharp.Core.Test.Types
{
    class DummyParent
    {
        [BinaryProperty(0)]
        public ushort A { get; set; }

        [BinaryProperty(1)]
        public Dummy B { get; set; }
    }
}