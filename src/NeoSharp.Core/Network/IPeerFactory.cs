using System.Threading.Tasks;

namespace NeoSharp.Core.Network
{
    public interface IPeerFactory
    {
        Task<IPeer> ConnectTo(EndPoint endPoint);
    }
}