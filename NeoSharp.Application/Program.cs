using NeoSharp.Client;
using SimpleInjector;

namespace NeoSharp.Application
{
    public class Program
    {
        static void Main(string[] args)
        {
            using (Container container = Composition.Compose())
            {
                IClientManager client = container.GetInstance<IClientManager>();
                client.RunClient(args);
            }
        }
    }
}