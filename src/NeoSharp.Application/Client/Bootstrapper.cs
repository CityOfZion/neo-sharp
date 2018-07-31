using NeoSharp.Core;

namespace NeoSharp.Application.Client
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Bootstrapper : IBootstrapper
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
        public Bootstrapper(IPrompt promptInit)
        {
            _prompt = promptInit;
        }

        /// <summary>
        /// Run client with arguments
        /// </summary>
        /// <param name="args">Arguments</param>
        public void Start(string[] args)
        {            
            _prompt.StartPrompt(args);
        }
    }
}