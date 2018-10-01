using System;
using System.Collections;
using System.IO;
using NeoSharp.BinarySerialization.SerializationHooks;

namespace NeoSharp.BinarySerialization.Serializers
{
    public class BinaryDictionarySerializer : IBinaryCustomSerializable
    {
        #region Private fields

        private readonly Type _type, _keyType, _valueType;
        private readonly IBinaryCustomSerializable _key;
        private readonly IBinaryCustomSerializable _value;

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
        /// <param name="type">Type</param>
        /// <param name="keyType">Key type</param>
        /// <param name="key">Key</param>
        /// <param name="valueType">Value type</param>
        /// <param name="value">Value</param>
        /// <param name="maxLength">Max length</param>
        public BinaryDictionarySerializer(Type type, Type keyType, IBinaryCustomSerializable key, Type valueType, IBinaryCustomSerializable value, int maxLength = ushort.MaxValue)
        {
            _type = type;

            if (_type.IsInterface)
            {
                // TODO #394: Compose a Dictionary<T> type
            }

            _keyType = keyType;
            _valueType = valueType;

            _key = key;
            _value = value;

            MaxLength = maxLength;
        }

        public int Serialize(IBinarySerializer serializer, BinaryWriter writer, object value, BinarySerializerSettings settings = null)
        {
            var ar = (IDictionary)value;

            if (ar == null)
            {
                return writer.WriteVarInt(0);
            }

            var x = writer.WriteVarInt(ar.Count);

            if (x > MaxLength) throw new FormatException(nameof(MaxLength));

            foreach (DictionaryEntry o in ar)
            {
                x += _key.Serialize(serializer, writer, o.Key, settings);
                x += _value.Serialize(serializer, writer, o.Value, settings);
            }

            return x;
        }

        public object Deserialize(IBinarySerializer deserializer, BinaryReader reader, Type type, BinarySerializerSettings settings = null)
        {
            var l = (int)reader.ReadVarInt(ushort.MaxValue);
            if (l > MaxLength) throw new FormatException(nameof(MaxLength));

            var a = (IDictionary)Activator.CreateInstance(_type);

            for (var ix = 0; ix < l; ix++)
            {
                a.Add(
                    _key.Deserialize(deserializer, reader, _keyType, settings),
                    _value.Deserialize(deserializer, reader, _valueType, settings)
                    );
            }

            return a;
        }
    }
}