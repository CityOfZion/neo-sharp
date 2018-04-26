using NeoSharp.Core.Network.Serialization;

namespace NeoSharp.Core.Network.Messages
{
    public interface ICarryPayload
    {
        IBinarySerializable Payload { get; }
    }
}