namespace NeoSharp.Core.Models.OperationManger
{
    public interface IVerifier<in T>
    {
        bool Verify(T obj);
    }
}
