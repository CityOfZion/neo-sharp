using System;
namespace NeoSharp.Core.Exceptions
{
	public class InvalidMessageException : Exception
    {
		public InvalidMessageException(string message) : base(message)
        {
        }

        public InvalidMessageException()
        {
        }
    }
}
