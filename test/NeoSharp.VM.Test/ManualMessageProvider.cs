namespace NeoSharp.VM.Test
{
    public class ManualMessageProvider : IMessageProvider
    {
        /// <summary>
        /// Message
        /// </summary>
        private readonly byte[] _message;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message</param>
        public ManualMessageProvider(byte[] message)
        {
            _message = message;
        }

        public byte[] GetMessage(uint iteration) => _message;
    }
}