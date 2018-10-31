namespace NeoSharp.Core.Messaging.Messages
{
    public class GetAddrMessage : Message
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public GetAddrMessage()
        {
            Command = MessageCommand.getaddr;
        }
    }
}