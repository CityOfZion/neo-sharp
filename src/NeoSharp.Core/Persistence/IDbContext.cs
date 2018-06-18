using System.Threading.Tasks;

namespace NeoSharp.Core.Persistence
{
    public interface IDbContext
    {
        Task Create(byte[] key, byte[] content);

        Task Delete(byte[] key);

        Task Update(byte[] key, byte[] content);

        Task<byte[]> GetByHash(byte[] hash);
    }
}
