namespace NeoSharp.Application.Client
{
    public class ClientManager : IClientManager
    {
        #region Variables

        /// <summary>
        /// Prompt
        /// </summary>
        private readonly IPrompt _prompt;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="promptInit">Prompt</param>
        public ClientManager(IPrompt promptInit)
        {
            _prompt = promptInit;
        }

        /// <summary>
        /// Run client with arguments
        /// </summary>
        /// <param name="args">Arguments</param>
        public void RunClient(string[] args)
        {            
            _prompt.StartPrompt(args);
        }
    }
}