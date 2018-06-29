using System.Threading.Tasks;

namespace NeoSharp.Core.Persistence
{
    public interface IDbBinaryContext
    {
        Task Create(byte[] key, byte[] content);

        Task Delete(byte[] key);

        Task Update(byte[] key, byte[] content);

        Task<byte[]> Get(byte[] key);
    }
}
