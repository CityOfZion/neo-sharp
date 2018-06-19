using System.Text;

namespace System.IO
{
    public static class BinaryWriterExtensions
    {
        public static int WriteVarBytes(this BinaryWriter writer, byte[] value)
        {
            int ret = writer.WriteVarInt(value.Length);
            writer.Write(value);
            return ret + value.Length;
        }

        public static int WriteVarInt(this BinaryWriter writer, long value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException();

            if (value < 0xFD)
            {
                writer.Write((byte)value);
                return 1;
            }
            else if (value <= 0xFFFF)
            {
                writer.Write((byte)0xFD);
                writer.Write((ushort)value);
                return 3;
            }
            else if (value <= 0xFFFFFFFF)
            {
                writer.Write((byte)0xFE);
                writer.Write((uint)value);
                return 5;
            }
            else
            {
                writer.Write((byte)0xFF);
                writer.Write(value);
                return 9;
            }
        }

        public static int WriteVarString(this BinaryWriter writer, string value)
        {
            return writer.WriteVarBytes(Encoding.UTF8.GetBytes(value));
        }
    }
}