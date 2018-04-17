namespace NeoSharp.Core.Network
{
    public interface IPeerFactory
    {
        IPeer Create(EndPoint endPoint);
    }
}