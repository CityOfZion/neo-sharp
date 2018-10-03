using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.Types;

namespace NeoSharp.Core.Blockchain.Repositories
{
    public class AssetRepository : IAssetRepository
    {
        #region Private Fields
        private readonly IRepository _repository;
        #endregion

        #region Constructor
        public AssetRepository(IRepository repository)
        {
            this._repository = repository;
        }
        #endregion

        #region IAssetModel implementation 

        /// <inheritdoc />
        public Task<Asset> GetAsset(UInt256 hash)
        {
            return this._repository.GetAsset(hash);
        }

        /// <inheritdoc />
        public Task<IEnumerable<Asset>> GetAssets()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}