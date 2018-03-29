namespace NeoSharp.Core.Database
{
    public interface IRepository
    {
        object GetBlock();
        void WriteBlock(object block);
    }
}
