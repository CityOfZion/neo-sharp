using NeoSharp.Core.Models;

namespace NeoSharp.Core.Blockchain.Genesis
{
    public interface IGenesisBuilder
    {
        Block Build();
    }
}