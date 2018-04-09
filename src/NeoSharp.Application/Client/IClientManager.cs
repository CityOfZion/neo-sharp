namespace NeoSharp.Application.Client
{
    public interface IClientManager
    {
        /// <summary>
        /// Run client with arguments
        /// </summary>
        /// <param name="args">Arguments</param>
        void RunClient(string[] args);
    }
}