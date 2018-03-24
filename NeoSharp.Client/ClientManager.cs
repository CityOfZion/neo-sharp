using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace NeoSharp.Client
{
    public class ClientManager : IClientManager
    {
        private readonly IPrompt _prompt;

        public ClientManager(IPrompt promptInit)
        {
            _prompt = promptInit;
        }

        public void RunClient(string[] args)
        {            
            _prompt.StartPrompt(args);
        }
    }
}

