namespace NeoSharp.Core.Messaging.Messages
{
    public class GetAddrMessage : Message
    {
        public GetAddrMessage()
        {
            Command = MessageCommand.getaddr;
        }
    }
}