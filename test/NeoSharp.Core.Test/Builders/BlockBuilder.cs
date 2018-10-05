using NeoSharp.Core.Models;
using NeoSharp.Types;

namespace NeoSharp.Core.Test.Builders
{
    public class BlockBuilder
    {
        public Block BuildGenerisBlock()
        {
            return new Block
            {
                Hash = UInt256.Parse("0xd42561e3d30e15be6400b6df2f328e02d2bf6354c41dce433bc57687c82144bf"),
                Index = 0
            };
        }
    }
}
