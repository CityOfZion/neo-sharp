using NeoSharp.Core.Network.Serialization;

namespace NeoSharp.Core.Network.Messages
{
    public class Message<TPayload> : Message, ICarryPayload
        where TPayload : IBinarySerializable, new()
    {
        public readonly TPayload Payload;

        protected Message()
        {
            Payload = new TPayload();
        }

        // public int Size => sizeof(uint) + 12 + sizeof(int) + sizeof(uint) + Payload.Length;

        IBinarySerializable ICarryPayload.Payload => Payload;
    }
}