using System;
using NeoSharp.BinarySerialization;
using NeoSharp.BinarySerialization.SerializationHooks;

namespace NeoSharp.Core.Messaging.Messages
{
    public class FilterLoadMessage : Message<FilterLoadPayload>
    {
        public FilterLoadMessage()
        {
            Command = MessageCommand.filterload;
            Payload = new FilterLoadPayload();
        }

        public FilterLoadMessage(byte[] filter, byte k, uint tweak)
        {
            if (k > 50) throw new FormatException();

            Command = MessageCommand.filterload;
            Payload = new FilterLoadPayload { Filter = filter, K = k, Tweak = tweak };
        }
    }

    public class FilterLoadPayload: IBinaryOnPostDeserializable
    {
        [BinaryProperty(0, MaxLength = 36000)]
        public byte[] Filter;

        [BinaryProperty(1)]
        public byte K;

        [BinaryProperty(2)]
        public uint Tweak;

        /// <summary>
        /// Validate
        /// </summary>
        public void OnPostDeserialize()
        {
            if (K > 50) throw new FormatException();
        }
    }
}