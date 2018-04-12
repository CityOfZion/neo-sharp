using System.IO;

namespace NeoSharp.Core.Types.Wrappers
{
    internal class ByteWrapper : SerializableWrapper<byte>
    {
        private byte _value;

        public override int Size => sizeof(byte);

        public ByteWrapper(byte value)
        {
            _value = value;
        }

        public override void Deserialize(BinaryReader reader)
        {
            _value = reader.ReadByte();
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(_value);
        }
    }
}
