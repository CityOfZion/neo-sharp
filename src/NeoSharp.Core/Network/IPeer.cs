namespace NeoSharp.Core.Network
{
    public interface IPeer
    {
        void Connect(uint serverNonce);
        void Disconnect();
    }    
}