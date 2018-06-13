using System;
using NeoSharp.BinarySerialization.SerializationHooks;

namespace NeoSharp.BinarySerialization
{
    public class BinaryTypeSerializerAttribute : Attribute
    {
        public readonly Type Type;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Type</param>
        public BinaryTypeSerializerAttribute(Type type)
        {
            if (!typeof(IBinaryCustomSerialization).IsAssignableFrom(type))
                throw new ArgumentException(nameof(type));

            Type = type;
        }

        /// <summary>
        /// Create serializer
        /// </summary>
        public IBinaryCustomSerialization Create()
        {
            return (IBinaryCustomSerialization)Activator.CreateInstance(Type);
        }
    }
}