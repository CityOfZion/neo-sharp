using System.Threading.Tasks;

namespace NeoSharp.Core.Blockchain
{
    public interface IBlockchain
    {
        /// <summary>
        /// Initialize the blockchain access.
        /// </summary>
        /// <returns>Task</returns>
        Task InitializeBlockchain();
    }
}