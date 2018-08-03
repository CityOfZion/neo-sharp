namespace NeoSharp.Application.Client
{
    public interface IPrompt
    {
        /// <summary>
        /// Execute command
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Return false if fail</returns>
        bool Execute(string command);
        
        /// <summary>
        /// Start prompt with arguments
        /// </summary>
        /// <param name="args">Arguments</param>
        void StartPrompt(string[] args);
    }
}