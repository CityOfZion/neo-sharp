namespace NeoSharp.Core.Models.OperationManager
{
    public interface ISigner<in T>
    {
        void Sign(T obj);
    }
}
