using System.Net;
using System.Threading.Tasks;

namespace NeoSharp.Network
{
    public interface IPeer
    {
        void Connect(IPEndPoint ipEP, uint serverNonce);
        void Stop();
    }

    public interface IPeerFactory
    {
        IPeer Create();
    }
}