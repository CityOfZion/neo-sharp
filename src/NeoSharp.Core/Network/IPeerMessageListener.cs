using System.Threading;

namespace NeoSharp.Core.Network
{
    public interface IPeerMessageListener
    {
        void StartFor(IPeer peer, CancellationToken cancellationToken);
    }
}
