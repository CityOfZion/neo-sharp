namespace NeoSharp.Core.Messaging
{
    public abstract class Message<TPayload> : Message, ICarryPayload where TPayload : new()
    {
        /// <summary>
        /// Payload
        /// </summary>
        public TPayload Payload { get; protected set; }

        object ICarryPayload.Payload
        {
            get => Payload;
            set => Payload = (TPayload)value;
        }
    }
}