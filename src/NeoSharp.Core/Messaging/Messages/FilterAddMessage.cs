using NeoSharp.BinarySerialization;

namespace NeoSharp.Core.Messaging.Messages
{
    public class FilterAddMessage : Message<FilterAddPayload>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public FilterAddMessage()
        {
            Command = MessageCommand.filteradd;
            Payload = new FilterAddPayload();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="payload">Payload</param>
        public FilterAddMessage(FilterAddPayload payload)
        {
            Command = MessageCommand.filteradd;
            Payload = payload;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">Data</param>
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