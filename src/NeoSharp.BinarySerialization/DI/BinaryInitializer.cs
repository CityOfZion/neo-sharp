using System;
namespace NeoSharp.BinarySerialization.DI
{
    public class BinaryInitializer : IBinaryInitializer
    {
        public BinaryInitializer(IBinarySerializer binarySerializer, IBinaryDeserializer binaryDeserializer)
        {
            BinarySerializer.Initialize(binarySerializer);
            BinaryDeserializer.Initialize(binaryDeserializer);
        }
    }
}
