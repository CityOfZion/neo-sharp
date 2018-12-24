namespace NeoSharp.VM
{
    public interface IMessageProvider
    {
        /// <summary>
        /// Get message for check signatures
        /// </summary>
        /// <param name="iteration">Iteration number</param>
        /// <returns>Return message content</returns>
        object GetMessage(uint iteration);

        /// <summary>
        /// Get message data for check signatures
        /// </summary>
        /// <param name="iteration">Iteration number</param>
        /// <returns>Return message data</returns>
        byte[] GetMessageData(uint iteration);
    }
}