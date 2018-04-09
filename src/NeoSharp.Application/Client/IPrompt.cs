namespace NeoSharp.Application.Client
{
    public interface IPrompt
    {
        /// <summary>
        /// Start prompt with arguments
        /// </summary>
        /// <param name="args">Arguments</param>
        void StartPrompt(string[] args);
    }
}