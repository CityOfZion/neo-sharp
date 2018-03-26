using System.Net;

namespace NeoSharp.Network
{
    public interface IPeer
    {
        void Connect(IPEndPoint ipEP, uint serverNonce);
        void Stop();
    }    
}