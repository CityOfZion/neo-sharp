using SimpleInjector;
using NeoSharp.Client;

namespace NeoSharp.Application
{
    public class Program
    {
        static void Main(string[] args)
        {
            Container container;
            Composition.Compose(out container);            

            IClientManager client = container.GetInstance<IClientManager>();
            client.RunClient(args);

            container.Dispose();
        }
    }
}
