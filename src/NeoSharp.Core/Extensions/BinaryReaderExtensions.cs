using System.IO;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Extensions
{
    public static class BinaryReaderExtensions
    {
        public static T ReadSerializable<T>(this BinaryReader reader) where T : ISerializable, new()
        {
            var obj = new T();
            obj.Deserialize(reader);
            return obj;
        }

        public static T[] ReadSerializableArray<T>(this BinaryReader reader, int max = 0x10000000) where T : ISerializable, new()
        {
            var array = new T[reader.ReadVarInt((ulong)max)];
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = new T();
                array[i].Deserialize(reader);
            }
            return array;
        }
    }
}