using System.Threading.Tasks;
using NeoSharp.Core.Models;

namespace NeoSharp.Core.Blockchain.Processing
{
    public interface IBlockPersister
    {
        Task Persist(params Block[] blocks);
    }
}
