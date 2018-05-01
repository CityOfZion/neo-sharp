namespace NeoSharp.Core.Network.Messaging
{
    public class Message<TPayload> : Message, ICarryPayload where TPayload : new()
    {
        /// <summary>
        /// Payload
        /// </summary>
        public TPayload Payload;

        object ICarryPayload.Payload => Payload;
    }
}