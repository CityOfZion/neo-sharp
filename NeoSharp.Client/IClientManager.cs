using SimpleInjector;

namespace NeoSharp.Client
{
    public interface IClientManager
    {
        void RunClient(string[] args);
    }
}