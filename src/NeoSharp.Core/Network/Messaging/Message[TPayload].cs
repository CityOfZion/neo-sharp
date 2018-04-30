using NeoSharp.Core.Serializers;

namespace NeoSharp.Core.Network.Messaging
{
    public class Message<TPayload> : Message, IBinaryOnPreSerializable, IBinaryOnPostDeserializable where TPayload : new()
    {
        /// <summary>
        /// Payload
        /// </summary>
        public TPayload Payload;

        /// <summary>
        /// Calculate raw data for the current payload
        /// </summary>
        public void OnPreSerialize()
        {
            RawPayload = BinarySerializer.Serialize(Payload);
        }
        /// <summary>
        /// Deserialize payload from RawData
        /// </summary>
        public void OnPostDeserialize()
        {
            Payload = BinarySerializer.Deserialize<TPayload>(RawPayload);
        }
    }
}