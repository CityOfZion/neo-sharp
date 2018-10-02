using System;
using System.Threading.Tasks;
using NeoSharp.Core.Models;

namespace NeoSharp.Core.Blockchain.Processing
{
    public interface IBlockHeaderPersister
    {
        BlockHeader LastBlockHeader { get; set; }

        event EventHandler<BlockHeader[]> OnBlockHeadersPersisted;

        Task Persist(params BlockHeader[] blockHeaders);
		Task Update(BlockHeader blockHeader);
	}
}