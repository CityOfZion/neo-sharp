using System;
namespace NeoSharp.Core.Exceptions
{
    public class InvalidEndpointException : FormatException
    {
        public InvalidEndpointException(string message) : base(message)
        {
        }
    }
}
