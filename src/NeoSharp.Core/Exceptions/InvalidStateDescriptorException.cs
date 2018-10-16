using System;
namespace NeoSharp.Core.Exceptions
{
	public class InvalidStateDescriptorException : FormatException
    {
        public InvalidStateDescriptorException()
        {
        }

        public InvalidStateDescriptorException(string message) : base(message)
        {
        }
    }
}
