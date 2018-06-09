namespace NeoSharp.VM
{
    public interface IMessageProvider
    {
        /// <summary>
        /// Get message for check signatures
        /// </summary>
        /// <param name="iteration">Iteration number</param>
        /// <returns>Return message content</returns>
        byte[] GetMessage(uint iteration);
    }
}