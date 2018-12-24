using System.Threading.Tasks;

namespace NeoSharp.Core.Models.OperationManager
{
    public interface IVerifier<in T>
    {
        Task<bool> Verify(T obj);
    }
}
