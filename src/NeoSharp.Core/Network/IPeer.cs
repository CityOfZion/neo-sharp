using System.Net;

namespace NeoSharp.Core.Network
{
    public interface IPeer
    {
        void Connect(IPEndPoint ipEP, uint serverNonce);
        void Stop();
    }    
}