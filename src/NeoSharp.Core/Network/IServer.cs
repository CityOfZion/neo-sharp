using System.Threading.Tasks;

namespace NeoSharp.Core.Network
{
    public interface IServer
    {
        Task StartServer();
        void StopServer();
    }
}