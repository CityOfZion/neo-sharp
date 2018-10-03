using System.Collections.Generic;
using System.Threading.Tasks;
using NeoSharp.Core.Models;
using NeoSharp.Types;

namespace NeoSharp.Core.Blockchain.Repositories
{
    public interface IAssetRepository
    {
        /// <summary>
        /// Return the corresponding asset information according to the specified hash
        /// </summary>
        /// <param name="hash">The asset hash.</param>
        /// <returns>The corresponding asset.</returns>
        Task<Asset> GetAsset(UInt256 hash);

        /// <summary>
        /// Return all assets
        /// </summary>
        /// <returns>List of all assets.</returns>
        Task<IEnumerable<Asset>> GetAssets();
    }
}
