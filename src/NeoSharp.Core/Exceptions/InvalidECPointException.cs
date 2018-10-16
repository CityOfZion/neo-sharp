using System;
namespace NeoSharp.Core.Exceptions
{
    public class InvalidECPointException : FormatException
    {
		public InvalidECPointException(string message) : base(message)
        {
        }
    }
}
