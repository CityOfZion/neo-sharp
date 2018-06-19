using System.IO;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Extensions
{
    public static class BinaryWriterExtensions
    {
        public static void Write(this BinaryWriter writer, ISerializable value)
        {
            value.Serialize(writer);
        }

        public static void Write<T>(this BinaryWriter writer, T[] value) where T : ISerializable
        {
            writer.WriteVarInt(value.Length);
            for (var i = 0; i < value.Length; i++)
            {
                value[i].Serialize(writer);
            }
        }
    }
}