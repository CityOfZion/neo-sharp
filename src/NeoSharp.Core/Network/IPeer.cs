using System.Net;

namespace NeoSharp.Core.Network
{
    public interface IPeer
    {
        void Connect(IPEndPoint ipEp, uint serverNonce);
        void Stop();
    }    
}