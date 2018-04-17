using System.Threading.Tasks;

namespace NeoSharp.Core.Network
{
    public interface IPeer
    {
        EndPoint EndPoint { get; }
        Task Connect(uint serverNonce);
        void Disconnect();
    }    
}