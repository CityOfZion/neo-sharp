using System;
namespace NeoSharp.Core.Exceptions
{
	public class BlockAlreadyQueuedException : Exception
    {
        public BlockAlreadyQueuedException(string message) : base(message)
        {
        }
    }
}
