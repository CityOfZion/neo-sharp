using NeoSharp.BinarySerialization;

namespace NeoSharp.Core.Messaging.Messages
{
    public class FilterAddMessage : Message<FilterAddPayload>
    {
        public FilterAddMessage()
        {
            Command = MessageCommand.filteradd;
            Payload = new FilterAddPayload();
        }

        public FilterAddMessage(byte[] data)
        {
            Command = MessageCommand.filteradd;
            Payload = new FilterAddPayload { Data = data };
        }
    }

    public class FilterAddPayload
    {
        [BinaryProperty(0, MaxLength = 520)]
        public byte[] Data;
    }
}