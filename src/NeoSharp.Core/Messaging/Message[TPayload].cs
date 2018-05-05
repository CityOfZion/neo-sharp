using System;

namespace NeoSharp.Core.Messaging
{
    public class Message<TPayload> : Message, ICarryPayload where TPayload : new()
    {
        /// <summary>
        /// Payload
        /// </summary>
        public TPayload Payload { get; protected set; }

        public Type PayloadType => typeof(TPayload);

        object ICarryPayload.Payload
        {
            get { return Payload; }
            set { Payload = (TPayload)value; }
        }
    }
}