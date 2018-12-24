namespace NeoSharp.VM.Test
{
    public class ManualMessageProvider : IMessageProvider
    {
        private readonly object _message;

        private readonly byte[] _messageData;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="messageData">Message data</param>
        public ManualMessageProvider(object message, byte[] messageData)
        {
            _message = message;
            _messageData = messageData;
        }

        public object GetMessage(uint iteration) => _message;

        public byte[] GetMessageData(uint iteration) => _messageData;
    }
}