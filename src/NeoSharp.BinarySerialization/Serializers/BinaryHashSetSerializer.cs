using System;
using System.Collections;
using System.IO;
using System.Reflection;
using NeoSharp.BinarySerialization.SerializationHooks;

namespace NeoSharp.BinarySerialization.Serializers
{
    public class BinaryHashSetSerializer : IBinaryCustomSerializable
    {
        #region Private fields

        private readonly Type _type, _itemType;
        private readonly MethodInfo _addMethod;
        private readonly PropertyInfo _countMethod;
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
        /// <param name="typeSet">Type set</param>
        /// <param name="serializer">Serializer</param>
        /// <param name="maxLength">Max length</param>
        public BinaryHashSetSerializer(Type typeSet, IBinaryCustomSerializable serializer, int maxLength = ushort.MaxValue)
        {
            _type = typeSet;
            _itemType = typeSet.GetElementType();

            if (_type.IsInterface)
            {
                // TODO #393: Compose a HashTag<T> type
            }

            MaxLength = maxLength;
            _serializer = serializer;
            _addMethod = typeSet.GetMethod("Add");
            _countMethod = typeSet.GetProperty("Count");
        }

        public int Serialize(IBinarySerializer serializer, BinaryWriter writer, object value, BinarySerializerSettings settings = null)
        {
            var ar = (IEnumerable)value;

            if (ar == null)
            {
                return writer.WriteVarInt(0);
            }

            var x = writer.WriteVarInt((int)_countMethod.GetValue(value));

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

            var a = Activator.CreateInstance(_type);

            for (var ix = 0; ix < l; ix++)
            {
                _addMethod.Invoke(a, new object[] { _serializer.Deserialize(deserializer, reader, _itemType, settings) });
            }

            return a;
        }
    }
}