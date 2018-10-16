using System;
namespace NeoSharp.Core.Exceptions
{
	public class InvalidBlockOrderException : Exception
    { 
        public InvalidBlockOrderException(string message) :base(message) 
        {
        }
    }
}
