namespace NeoSharp.Core.Persistence
{
    public interface IRepository
    {
        object GetBlock();
        void WriteBlock(object block);
    }
}
