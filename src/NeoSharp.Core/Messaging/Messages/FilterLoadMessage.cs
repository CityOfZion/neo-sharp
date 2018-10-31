using System;
using NeoSharp.BinarySerialization;
using NeoSharp.BinarySerialization.SerializationHooks;

namespace NeoSharp.Core.Messaging.Messages
{
    public class FilterLoadMessage : Message<FilterLoadPayload>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public FilterLoadMessage()
        {
            Command = MessageCommand.filterload;
            Payload = new FilterLoadPayload();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="payload">Payload</param>
        public FilterLoadMessage(FilterLoadPayload payload)
        {
            Command = MessageCommand.filterload;
            Payload = payload;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="k">k</param>
        /// <param name="tweak">Tweak</param>
        public FilterLoadMessage(byte[] filter, byte k, uint tweak)
        {
            if (k > 50) throw new FormatException();

            Command = MessageCommand.filterload;
            Payload = new FilterLoadPayload { Filter = filter, K = k, Tweak = tweak };
        }
    }

    public class FilterLoadPayload : IBinaryVerifiable
    {
        [BinaryProperty(0, MaxLength = 36000)]
        public byte[] Filter;

        [BinaryProperty(1)]
        public byte K;

        [BinaryProperty(2)]
        public uint Tweak;

        /// <summary>
        /// Verificable
        /// </summary>
        public bool Verify()
        {
            if (K > 50) throw new ArgumentException(nameof(K));

            return true;
        }
    }
}