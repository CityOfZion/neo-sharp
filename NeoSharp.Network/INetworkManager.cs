using System;
using System.Threading.Tasks;

namespace NeoSharp.Network
{
    public interface INetworkManager
    {
        void StartNetwork();
        void StopNetwork();
    }
}