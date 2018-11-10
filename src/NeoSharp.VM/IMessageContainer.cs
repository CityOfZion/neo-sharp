namespace NeoSharp.VM
{
    public interface IMessageContainer : IMessageProvider
    {
        void RegisterMessage(byte[] message);
    }
}