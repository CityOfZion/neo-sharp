namespace NeoSharp.Core.Messaging.Messages
{
    public class FilterClearMessage : Message
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public FilterClearMessage()
        {
            Command = MessageCommand.filterclear;
        }
    }
}