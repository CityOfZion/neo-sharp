using System;
using System.Collections;
using System.IO;
using NeoSharp.BinarySerialization.SerializationHooks;

namespace NeoSharp.BinarySerialization.Serializers
{
    public class BinaryListSerializer : IBinaryCustomSerializable
    {
        #region Private fields

        private readonly Type _type, _itemType;
        private readonly IBinaryCustomSerializable _serializer;

        #endregion

        #region Public fields

        /// <summary>
        /// Max length
        /// </summary>
        public readonly int MaxLength;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="typeList">Type list</param>
        /// <param name="serializer">Serializer</param>
        /// <param name="maxLength">Max length</param>
        public BinaryListSerializer(Type typeList, IBinaryCustomSerializable serializer, int maxLength = ushort.MaxValue)
        {
            _type = typeList;
            _itemType = typeList.GetElementType();
            _serializer = serializer;
            MaxLength = maxLength;
        }

        public int Serialize(IBinarySerializer serializer, BinaryWriter writer, object value, BinarySerializerSettings settings = null)
        {
            var ar = (IList)value;

            if (ar == null)
            {
                return writer.WriteVarInt(0);
            }

            var x = writer.WriteVarInt(ar.Count);

            if (x > MaxLength) throw new FormatException(nameof(MaxLength));

            foreach (var o in ar)
            {
                x += _serializer.Serialize(serializer, writer, o, settings);
            }

            return x;
        }

        public object Deserialize(IBinarySerializer deserializer, BinaryReader reader, Type type, BinarySerializerSettings settings = null)
        {
            var l = (int)reader.ReadVarInt(ushort.MaxValue);
            if (l > MaxLength) throw new FormatException(nameof(MaxLength));

            var a = (IList)Activator.CreateInstance(_type);

            for (var ix = 0; ix < l; ix++)
            {
                a.Add(_serializer.Deserialize(deserializer, reader, _itemType, settings));
            }

            return a;
        }
    }
}