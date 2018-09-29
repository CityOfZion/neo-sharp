namespace NeoSharp.Core.Models.OperationManger
{
    public interface ISigner<in T>
    {
        void Sign(T obj);
    }
}
