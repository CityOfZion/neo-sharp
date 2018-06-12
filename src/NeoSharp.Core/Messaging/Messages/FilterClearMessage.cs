namespace NeoSharp.Core.Messaging.Messages
{
    public class FilterClearMessage : Message
    {
        public FilterClearMessage()
        {
            Command = MessageCommand.filterclear;
        }
    }
}