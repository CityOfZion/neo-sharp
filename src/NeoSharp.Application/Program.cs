using NeoSharp.Application.DI;

namespace NeoSharp.Application
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var applicationBootstrapper = new ApplicationBootstrapper();

            applicationBootstrapper.RegisterModules();
            applicationBootstrapper.Start(args);
        }
    }
}